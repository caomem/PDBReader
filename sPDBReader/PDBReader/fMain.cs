using PDBReader.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

namespace PDBReader
{
    /// <summary>
    /// Main Form for convert a PDB File in Distances File
    /// </summary>
    public partial class fMain : Form
    {
        #region Components

        /// <summary>
        /// List of atoms extracted of pdb file
        /// </summary>
        private List<Atom> atoms = new List<Atom>();

        #endregion

        /// <summary>
        /// fMain Constructor
        /// </summary>
        public fMain()
        {
            InitializeComponent();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.AllKeys.Contains("fileDir"))
            {
                cbPDBFile.Text = config.AppSettings.Settings["fileDir"].Value;
            }
            if (config.AppSettings.Settings.AllKeys.Contains("fileConDir"))
            {
                cbConvertFile.Text = config.AppSettings.Settings["fileConDir"].Value;
            }

            if (!string.IsNullOrWhiteSpace(cbPDBFile.Text) && !string.IsNullOrWhiteSpace(cbConvertFile.Text))
            {
                btConvert.Enabled = true;
            }

            btNear.Tag = false;
            cbOutput.SelectedItem = cbOutput.Items[0];
        }

        /// <summary>
        /// Click event of btOpen
        /// Open select file dialog for select PDB file
        /// </summary>
        private void btOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PDB files (*.ent, *.pdb)|*.ent;*.pdb|All files (*.*)|*.*";
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a PDB file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                cbPDBFile.Text = dialog.FileName;
                btConvert.Enabled = true;
                cbConvertFile.Text = dialog.FileName.Remove(dialog.FileName.Length - 3, 3) + "pdb";
            }
        }

        /// <summary>
        /// Click event of btConvert
        /// Convert selected PDB file to Distances file
        /// </summary>
        private void btConvert_Click(object sender, EventArgs e)
        {
            atoms.Clear();
            if (String.IsNullOrWhiteSpace(cbPDBFile.Text))
            {
                MessageBox.Show("Please select a valid PDB file", "Alert");
                btConvert.Enabled = false;
                return;
            }
            string[] text = null;
            try
            {
                text = System.IO.File.ReadAllLines(cbPDBFile.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error when read the file");
                return;
            }

            List<Exception> exceptions = new List<Exception>();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].StartsWith("ATOM  ") || text[i].StartsWith("HETATM"))
                {
                    if (!string.IsNullOrWhiteSpace(txtFilter.Text) && !text[i].Substring(12, 4).Contains(txtFilter.Text)) continue;
                    if (!string.IsNullOrWhiteSpace(txtResidue.Text) && !text[i].Substring(17, 3).Contains(txtResidue.Text)) continue;
                    Atom a = new Atom();
                    try
                    {
                        a.Serial = int.Parse(text[i].Substring(6, 5));
                        if (!cbToy.Checked) // Accept repeted atoms in toy problems
                            foreach (var aux in atoms) //don't repeat atoms (other models will also not be read)
                            {
                                if (a.Serial == aux.Serial) goto end;
                            }
                        a.Name = text[i].Substring(12, 4).Trim();
                        a.AltLoc = text[i].Length > 16 ? text[i][16] : ' ';
                        a.ResName = text[i].Substring(17, 3);
                        a.ChainID = text[i].Length > 21 ? text[i][21] : ' ';
                        a.ResSeq = int.Parse(text[i].Substring(22, 4));
                        a.ICode = text[i].Length > 26 ? text[i][26] : ' ';
                        a.X = double.Parse(text[i].Substring(30, 8), new CultureInfo("en-US"));
                        a.Y = double.Parse(text[i].Substring(38, 8), new CultureInfo("en-US"));
                        a.Z = double.Parse(text[i].Substring(46, 8), new CultureInfo("en-US"));
                        a.Occupancy = double.Parse(text[i].Substring(54, 6), new CultureInfo("en-US"));
                        a.TempFactor = double.Parse(text[i].Substring(60, 6), new CultureInfo("en-US"));
                        a.Element = text[i].Substring(76, 2).Trim();
                        a.Charge = text[i].Substring(78, 2).Trim();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                        continue;
                    }
                    if (txtResId.Value == 0 || txtResId.Value == a.ResSeq) atoms.Add(a);
                }
            end: { }
            }
            {
                int i = 0;
                foreach (var ex in exceptions)
                {
                    MessageBox.Show(ex.ToString(), "Error " + ((++i).ToString()) + " when list the atoms in file");
                }
            }

            if (cbHcOrder.Checked)
            {
                if (atoms.Count == 0) return;
                List<Atom> chainReOrder = new List<Atom>(), hetAtoms = new List<Atom>(), currentResidue = new List<Atom>(), previousResidue = new List<Atom>();
                int currentRes = atoms.First().ResSeq;
                bool ended = false;
                foreach (var atom in atoms)
                {
                    if (atom.ResSeq == currentRes)
                    {
                        currentResidue.Add(atom);
                        if (atoms.Last() != atom)
                        {
                            continue;
                        }
                    }
                    try
                    {
                        AminoAcidName amino = currentResidue.First().GetAminoAcidName();
                        if (currentResidue.Find(a => a.Name == AminoAcidAtom.H2.GetPDBEquivalent()) != null)
                        {
                            Atom aux;
                            if ((aux = currentResidue.Find(a => a.Name == AminoAcidAtom.N.GetPDBEquivalent())) != null)
                                chainReOrder.Add(aux);
                            else throw new Exception("Não tem " + AminoAcidAtom.N.GetPDBEquivalent());
                            if ((amino == AminoAcidName.Proline && (aux = currentResidue.Find(a => a.Name == "HD2")) != null)
                                || ((aux = currentResidue.Find(a => a.Name == "H" || a.Name == "H1")) != null))
                                chainReOrder.Add(aux);
                            else throw new Exception("Não tem H");
                            if ((aux = currentResidue.Find(a => a.Name == AminoAcidAtom.H2.GetPDBEquivalent())) != null)
                                chainReOrder.Add(aux);
                            else throw new Exception("Não tem " + AminoAcidAtom.H2.GetPDBEquivalent());
                            if ((aux = currentResidue.Find(a => a.Name == "CA")) != null)
                                chainReOrder.Add(aux);
                            else throw new Exception("Não tem CA");
                            if ((aux = currentResidue.Find(a => a.Name == AminoAcidAtom.N.GetPDBEquivalent())) != null)
                                chainReOrder.Add(aux);
                            else throw new Exception("Não tem " + AminoAcidAtom.N.GetPDBEquivalent());
                            if ((amino == AminoAcidName.Glycine && (aux = currentResidue.Find(a => a.Name == "HA2")) != null)
                                || (aux = currentResidue.Find(a => a.Name == "HA")) != null)
                                chainReOrder.Add(aux);
                            else throw new Exception("Não tem HA");
                            if ((aux = currentResidue.Find(a => a.Name == "C")) != null)
                                chainReOrder.Add(aux);
                            else throw new Exception("Não tem C");
                            if ((aux = currentResidue.Find(a => a.Name == "CA")) != null)
                                chainReOrder.Add(aux);
                            else throw new Exception("Não tem CA");
                        }
                        else
                        {
                            if (chainReOrder.Count == 0) throw new Exception("Beginning of molecule is not good defined, please, verify the PDB instance");
                            if (currentResidue.Find(a => a.Name == "OXT") != null)
                            {
                                Atom aux;
                                if ((amino == AminoAcidName.Proline && (aux = currentResidue.Find(a => a.Name == "HD2")) != null)
                                    || (aux = currentResidue.Find(a => a.Name == "H")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem H");
                                if ((aux = currentResidue.Find(a => a.Name == "CA")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem CA");
                                if ((aux = previousResidue.Find(a => a.Name == "O")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem O previo");
                                if ((aux = currentResidue.Find(a => a.Name == AminoAcidAtom.N.GetPDBEquivalent())) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem " + AminoAcidAtom.N.GetPDBEquivalent());
                                if ((amino == AminoAcidName.Proline && (aux = currentResidue.Find(a => a.Name == "HD2")) != null)
                                    || (aux = currentResidue.Find(a => a.Name == "H")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem H");
                                if ((aux = currentResidue.Find(a => a.Name == "CA")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem CA");
                                if ((aux = currentResidue.Find(a => a.Name == AminoAcidAtom.N.GetPDBEquivalent())) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem " + AminoAcidAtom.N.GetPDBEquivalent());
                                if ((amino == AminoAcidName.Glycine && (aux = currentResidue.Find(a => a.Name == "HA2")) != null)
                                    || (aux = currentResidue.Find(a => a.Name == "HA")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem HA");
                                if ((aux = currentResidue.Find(a => a.Name == "C")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem C");
                                if ((aux = currentResidue.Find(a => a.Name == "CA")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem CA");
                                if ((aux = currentResidue.Find(a => a.Name == "OXT")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem OXT");
                                if ((aux = currentResidue.Find(a => a.Name == "C")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem C");
                                if ((aux = currentResidue.Find(a => a.Name == "O")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem O");
                                ended = true;
                            }
                            else
                            {
                                Atom aux;
                                if ((amino == AminoAcidName.Proline && (aux = currentResidue.Find(a => a.Name == "HD2")) != null)
                                    || (aux = currentResidue.Find(a => a.Name == "H")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem H");
                                if ((aux = currentResidue.Find(a => a.Name == "CA")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem CA");
                                if ((aux = previousResidue.Find(a => a.Name == "O")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem O previo");
                                if ((aux = currentResidue.Find(a => a.Name == AminoAcidAtom.N.GetPDBEquivalent())) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem " + AminoAcidAtom.N.GetPDBEquivalent());
                                if ((amino == AminoAcidName.Proline && (aux = currentResidue.Find(a => a.Name == "HD2")) != null)
                                    || (aux = currentResidue.Find(a => a.Name == "H")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem H");
                                if ((aux = currentResidue.Find(a => a.Name == "CA")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem CA");
                                if ((aux = currentResidue.Find(a => a.Name == AminoAcidAtom.N.GetPDBEquivalent())) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem " + AminoAcidAtom.N.GetPDBEquivalent());
                                if ((amino == AminoAcidName.Glycine && (aux = currentResidue.Find(a => a.Name == "HA2")) != null)
                                    || (aux = currentResidue.Find(a => a.Name == "HA")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem HA");
                                if ((aux = currentResidue.Find(a => a.Name == "C")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem C");
                                if ((aux = currentResidue.Find(a => a.Name == "CA")) != null)
                                    chainReOrder.Add(aux);
                                else throw new Exception("Não tem CA");
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        if (txtResId.Value != 0)
                        {
                            MessageBox.Show("Dont's possible to calc the HC Order for a non padronized protein.\n Please, dont use the HC Order with Res filter.", "Warning");
                            break;
                        }
                        else if (ended)
                        {
                            MessageBox.Show("The reorder is completed, but an error occurs.\n This molecule has het atoms?\n" + er.Message, "Warning in HC Order calc");
                            break;
                        }
                        else
                        {
                            MessageBox.Show(er.Message, "HC Order calc Error");
                            break;
                        }
                    }
                    
                    previousResidue = new List<Atom>(currentResidue);
                    currentResidue.Clear();
                    currentRes = atom.ResSeq;
                    currentResidue.Add(atom);
                }
                atoms = new List<Atom>(chainReOrder);
                for (int i = 3; i < atoms.Count; i++)
                {
                    if (Math.Pow(atoms.ElementAt(i - 3).Distance(atoms.ElementAt(i - 2)) + atoms.ElementAt(i - 2).Distance(atoms.ElementAt(i - 1)) - atoms.ElementAt(i - 3).Distance(atoms.ElementAt(i - 1)), 2) < 0.001 
                        || atoms.ElementAt(i-3).Distance(atoms.ElementAt(i-1)) > atoms.ElementAt(i-3).Distance(atoms.ElementAt(i-2)) + atoms.ElementAt(i - 2).Distance(atoms.ElementAt(i - 1)))
                    {
                        MessageBox.Show("Atoms "+(i-3)+", "+(i-2)+" and "+(i-1)+" are coplanar\n\t d(i-3,i-1) <= d(i-3,i-2) + d(i-2,i-1) => "+ 
                            atoms.ElementAt(i - 3).Distance(atoms.ElementAt(i - 1)) + " <= "+ atoms.ElementAt(i - 3).Distance(atoms.ElementAt(i - 2)) + " + " + atoms.ElementAt(i - 2).Distance(atoms.ElementAt(i - 1)) +
                            "\n"+atoms.ElementAt(i-3)+"\n"+atoms.ElementAt(i-1),"Error");
                        return;
                    }
                }
            }

            if (cbView.Checked)
            {
                btBefore.Visible = true;
                btBefore.Enabled = false;
                btAfter.Visible = true;
                btDetails.Visible = true;
                btNear.Visible = true;
                btHc.Visible = true;

                pnVisualization.Tag = 0;
                if (atoms.Count == 0)
                {
                    btAfter.Enabled = false;
                    btDetails.Enabled = false;
                    return;
                }
                else
                {
                    btAfter.Enabled = true;
                    btDetails.Enabled = true;
                }

                updateView(atoms.ElementAt(0));
            }
            else
            {
                btBefore.Visible = false;
                btBefore.Enabled = false;
                btAfter.Visible = false;
                btDetails.Visible = false;
                btNear.Visible = false;
                btHc.Visible = false;

                Graphics graphics = pnVisualization.CreateGraphics();
                graphics.Clear(Color.Gray);

                if (string.IsNullOrWhiteSpace(cbConvertFile.Text)) return;
                if (cbOutput.SelectedItem == cbOutput.Items[0]) //XML
                {
                    if (cbBP.Checked)
                    {
                        MessageBox.Show("Sorry, BP is not implemented for XML", "Alert");
                    }
                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        XmlSerializer serializer = new XmlSerializer(atoms.GetType());
                        using (MemoryStream stream = new MemoryStream())
                        {
                            serializer.Serialize(stream, atoms);
                            stream.Position = 0;
                            xmlDocument.Load(stream);
                            xmlDocument.Save(cbConvertFile.Text);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else if (cbOutput.SelectedItem == cbOutput.Items[1]) // MATRIX
                {
                    if (cbBP.Checked)
                    {
                        List<Atom> atomsSingle = new List<Atom>();
                        for (int i = 0; i < atoms.Count; i++)
                        {
                            if (atoms.IndexOf(atoms.ElementAt(i)) < i) continue;
                            atomsSingle.Add(atoms.ElementAt(i));
                        }
                        StringBuilder file = new StringBuilder();
                        file.Append("E = [");
                        bool first = true;
                        foreach (Atom ai in atoms)
                        {
                            if (first) first = false; else file.Append(";");
                            foreach (Atom aj in atoms)
                            {
                                if (isValidDistance(ai, aj))
                                    //file.Append(" {" + atomsSingle.IndexOf(ai) + ", " + atomsSingle.IndexOf(aj) + " " + ai.Distance(aj).ToString(new CultureInfo("en-US")) + "}");
                                    file.Append(" " + ai.Distance(aj).ToString(new CultureInfo("en-US")));
                                else file.Append(" 0");
                            }
                        }
                        file.Append("]");
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                    else
                    {
                        StringBuilder file = new StringBuilder();
                        file.Append("PDB = [");
                        bool first = true;
                        foreach (Atom a in atoms)
                        {
                            if (first) first = false; else file.Append(";");
                            file.Append(a.X.ToString(new CultureInfo("en-US")) + " " + a.Y.ToString(new CultureInfo("en-US")) + " " + a.Z.ToString(new CultureInfo("en-US")));
                        }
                        file.Append("]");
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                }
                else if (cbOutput.SelectedItem == cbOutput.Items[2]) // JSON
                {
                    if (cbBP.Checked)
                    {
                        List<Atom> atomsSingle = new List<Atom>();
                        for (int i = 0; i < atoms.Count; i++)
                        {
                            if (atoms.IndexOf(atoms.ElementAt(i)) < i) continue;
                            atomsSingle.Add(atoms.ElementAt(i));
                        }
                        StringBuilder file = new StringBuilder();
                        file.Append("{\"BP\":{");
                        file.Append("V:[");
                        bool first = true;
                        foreach (Atom a in atoms)
                        {
                            if (first) first = false; else file.Append(",");
                            file.Append("{\"Vi\":\"" + "v" + atomsSingle.IndexOf(a) + "\"}");
                        }
                        file.Append("],E:[");
                        first = true;
                        int current = 0;
                        for (int i = 0; i < atomsSingle.Count; i++)
                        {
                            current = atoms.IndexOf(atomsSingle.ElementAt(i));
                            for (int j = 1; j < 4 && i - j >= 0; j++)
                            {
                                if (first) first = false; else file.Append(",");
                                file.Append("{\"v" + i + ",v" + atomsSingle.IndexOf(atoms.ElementAt(current - j)) +
                                    "\":" + atomsSingle.ElementAt(i).Distance(atoms.ElementAt(current - j)).ToString(new CultureInfo("en-US")) + "}");
                            }
                        }
                        file.Append("]}}");
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                    else
                    {
                        StringBuilder file = new StringBuilder();
                        file.Append("{\"atoms\":[");
                        bool first = true;
                        foreach (Atom a in atoms)
                        {
                            if (first) first = false; else file.Append(",");
                            file.Append("{\"name\":\"" + a.Name + "\",\"position\":{\"x\":" + a.X.ToString(new CultureInfo("en-US")) + ",\"y\":" + a.Y.ToString(new CultureInfo("en-US")) + ",\"z\":" + a.Z.ToString(new CultureInfo("en-US")) + "}}");
                        }
                        file.Append("]}");
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                }
                else if (cbOutput.SelectedItem == cbOutput.Items[3]) // MOLECULAR CONFORMATION
                {
                    if (cbBP.Checked)
                    {
                        List<Atom> atomsSingle = new List<Atom>();
                        for (int i = 0; i < atoms.Count; i++)
                        {
                            if (atoms.IndexOf(atoms.ElementAt(i)) < i) continue;
                            atomsSingle.Add(atoms.ElementAt(i));
                        }
                        StringBuilder file = new StringBuilder();
                        bool first = true;
                        foreach (Atom ai in atoms)
                        {
                            if (first) first = false; else file.Append("\r\n");
                            bool first2 = true;
                            foreach (Atom aj in atoms)
                            {
                                if (first2) first2 = false; else file.Append("\t");
                                file.Append(ai.Distance(aj).ToString(new CultureInfo("en-US")));
                            }
                        }
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                    else
                    {
                        StringBuilder file = new StringBuilder();
                        bool first = true;
                        foreach (Atom a in atoms)
                        {
                            if (first) first = false; else file.Append("\r\n");
                            file.Append(a.X.ToString(new CultureInfo("en-US")) + " " + a.Y.ToString(new CultureInfo("en-US")) + " " + a.Z.ToString(new CultureInfo("en-US")));
                        }
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                }
                else if (cbOutput.SelectedItem == cbOutput.Items[4]) // XYZ
                {
                    if (cbBP.Checked)
                    {
                        MessageBox.Show("Sorry, BP is not implemented for xyz", "Alert");
                    }
                    else
                    {
                        List<Atom> validAtoms = new List<Atom>(); //Code for select single atoms
                        foreach (var a in atoms)
                        {
                            if (!validAtoms.Exists(temp => temp.Equals(a))) validAtoms.Add(a);
                        }
                        StringBuilder file = new StringBuilder();
                        file.Append(validAtoms.Count.ToString()+"\n\n"); //atoms
                        bool first = true;
                        foreach (Atom a in validAtoms) //atoms
                        {
                            if (first) first = false; else file.Append("\n");
                            file.Append(a.Name+"\t"+ a.X.ToString(new CultureInfo("en-US")) + "\t" + a.Y.ToString(new CultureInfo("en-US")) + "\t" + a.Z.ToString(new CultureInfo("en-US")));
                        }
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                }
                else if (cbOutput.SelectedItem == cbOutput.Items[5]) // MD-Jeep
                {
                    if (!cbBP.Checked)
                    {
                        MessageBox.Show("Only BP configuration is implemented for MD-jeep", "Alert");
                    }
                    StringBuilder file = new StringBuilder();
                    bool first = true;
                    Atom[] previusAtoms = new Atom[] { null, null, null };
                    if (cbHcOrder.Checked || cbToy.Checked)
                    {
                        List<Atom> atomsSingle = new List<Atom>();
                        //for (int i = 0; i < atoms.Count; i++)
                        //{
                        //   if (atoms.IndexOf(atoms.ElementAt(i)) < i) continue;
                        //    atomsSingle.Add(atoms.ElementAt(i));
                        //}
                        for (int i = 0; i < atoms.Count; i++)
                        {
                            if (atoms.IndexOf(atoms.ElementAt(i)) < i)
                            {
                                previusAtoms[0] = previusAtoms[1];
                                previusAtoms[1] = previusAtoms[2];
                                previusAtoms[2] = atoms.ElementAt(i);
                                continue;
                            }
                            atomsSingle.Add(atoms.ElementAt(i));

                            List<Atom> validAtoms = new List<Atom>();
                            foreach (Atom atom in previusAtoms)
                            {
                                if (atom != null)
                                {
                                    validAtoms.Add(atom);
                                }
                            }
                            for (int j = 0; j < i-3; j++)
                            {
                                if ( atoms.ElementAt(i).Element == "H" && atoms.ElementAt(j).Element == "H" && validAtoms.IndexOf(atoms.ElementAt(j)) == -1 && isValidDistance(atoms.ElementAt(j), atoms.ElementAt(i)))
                                {
                                    validAtoms.Add(atoms.ElementAt(j));
                                }
                            }
                            foreach (Atom validAtom in validAtoms)
                            {
                                if (first) first = false; else file.Append("\r\n");
                                file.Append((atomsSingle.IndexOf(atoms.ElementAt(i)) + 1).ToString().PadLeft(5, ' ') + (atomsSingle.IndexOf(validAtom) + 1).ToString().PadLeft(5, ' ') + " " +
                                    atoms.ElementAt(i).ResSeq.ToString().PadLeft(5, ' ') + validAtom.ResSeq.ToString().PadLeft(5, ' ') + " " +
                                    atoms.ElementAt(i).Distance(validAtom).ToString(new CultureInfo("en-US")).PadLeft(21, ' ') +
                                    atoms.ElementAt(i).Distance(validAtom).ToString(new CultureInfo("en-US")).PadLeft(21, ' ') + "  " +
                                    atoms.ElementAt(i).Name.PadRight(4, ' ') + " " + validAtom.Name.PadRight(4, ' ') + "  " +
                                    atoms.ElementAt(i).ResName.PadRight(4, ' ') + " " + validAtom.ResName.PadRight(4, ' '));
                            }

                            previusAtoms[0] = previusAtoms[1];
                            previusAtoms[1] = previusAtoms[2];
                            previusAtoms[2] = atoms.ElementAt(i);
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (Atom ai in atoms)
                        {
                            int j = 0;
                            foreach (Atom aj in atoms)
                            {
                                if (first) first = false; else file.Append("\r\n");
                                file.Append((i+1).ToString().PadLeft(5, ' ') + (j + 1).ToString().PadLeft(5, ' ') + " " +
                                    atoms.ElementAt(i).ResSeq.ToString().PadLeft(5, ' ') + atoms.ElementAt(j).ResSeq.ToString().PadLeft(5, ' ') + " " +
                                    atoms.ElementAt(i).Distance(atoms.ElementAt(j)).ToString(new CultureInfo("en-US")).PadLeft(21, ' ') +
                                    atoms.ElementAt(i).Distance(atoms.ElementAt(j)).ToString(new CultureInfo("en-US")).PadLeft(21, ' ') + "  " +
                                    atoms.ElementAt(i).Name.PadRight(4, ' ') + " " + atoms.ElementAt(j).Name.PadRight(4, ' ') + "  " +
                                    atoms.ElementAt(i).ResName.PadRight(4, ' ') + " " + atoms.ElementAt(j).ResName.PadRight(4, ' '));
                                j++;
                            }
                            i++;
                        }
                    }

                    File.WriteAllText(cbConvertFile.Text, file.ToString());
                }
                else if (cbOutput.SelectedItem == cbOutput.Items[6]) // Virtual Order
                {
                    if (cbBP.Checked)
                    {
                        List<Atom> atomsSingle = new List<Atom>();
                        for (int i = 0; i < atoms.Count; i++)
                        {
                            if (atoms.IndexOf(atoms.ElementAt(i)) < i) continue;
                            atomsSingle.Add(atoms.ElementAt(i));
                        }
                        StringBuilder file = new StringBuilder();
                        bool first = true;
                        foreach (Atom atom in atoms)
                        {
                            if (first) first = false; else file.Append(" ");
                            file.Append((atomsSingle.IndexOf(atom)+1).ToString());
                        }
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                }
            }
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.AllKeys.Contains("fileDir"))
            {
                config.AppSettings.Settings["fileDir"].Value = cbPDBFile.Text;
            }
            else
            {
                config.AppSettings.Settings.Add("fileDir", cbPDBFile.Text);
            }
            if (config.AppSettings.Settings.AllKeys.Contains("fileConDir"))
            {
                config.AppSettings.Settings["fileConDir"].Value = cbConvertFile.Text;
            }
            else
            {
                config.AppSettings.Settings.Add("fileConDir", cbConvertFile.Text);
            }
            config.Save(ConfigurationSaveMode.Full);
        }

        /// <summary>
        /// Changed event of check box cbView
        /// Enable atomic preview
        /// </summary>
        private void cbView_CheckedChanged(object sender, EventArgs e)
        {
            if (cbView.Checked)
            {
                tbScale.Enabled = true;
                cbInstante.Enabled = true;
                cbChainMark.Enabled = true;
                cbOutput.Enabled = false;
                cbBP.Enabled = false;
            }
            else
            {
                tbScale.Enabled = false;
                cbInstante.Enabled = false;
                cbChainMark.Enabled = false;
                cbOutput.Enabled = true;
                cbBP.Enabled = true;
            }
        }

        /// <summary>
        /// Click event of button btDetails
        /// Show details of current atom
        /// </summary>
        private void btDetails_Click(object sender, EventArgs e)
        {
            List<Atom> validAtoms = new List<Atom>();
            Atom atom = atoms.ElementAt((int)pnVisualization.Tag);
            foreach (var a in atoms)
            {
                if (a.Equals(atom)) continue;
                if (validAtoms.Exists(temp => temp.Equals(a))) continue;
                if (isValidDistance(atom, a) && (txtResId.Value == 0 || a.ResSeq == txtResId.Value))
                {
                    validAtoms.Add(a);
                }
            }
            MessageBox.Show("Count of atoms: " + validAtoms.Count + "\nCurrent Atom = " + atom.ToString(), "Details of the current atom");
            if (validAtoms.Count > 0 && validAtoms.Count < 50)
            {
                string atts = "  ";
                foreach (var att in validAtoms)
                {
                    atts += att.ToString() + "  dis = " + atom.Distance(att).ToString() + "\n   ";
                }
                MessageBox.Show("\n" + atts, "Details of nearby atoms");
            }
        }

        /// <summary>
        /// Click event of button btAfter
        /// Switch to the next atom
        /// </summary>
        private void btAfter_Click(object sender, EventArgs e)
        {
            if (!btBefore.Enabled) btBefore.Enabled = true;
            pnVisualization.Tag = (int)pnVisualization.Tag + 1;
            if ((int)pnVisualization.Tag == atoms.Count - 1) btAfter.Enabled = false;

            if (txtResId.Value != 0)
            {
                txtResId.Value = atoms.ElementAt((int)pnVisualization.Tag).ResSeq;
            }
            updateView(atoms.ElementAt((int)pnVisualization.Tag));
        }

        /// <summary>
        /// Click event of button btBefore
        /// Switch to the previous atom
        /// </summary>
        private void btBefore_Click(object sender, EventArgs e)
        {
            if (!btAfter.Enabled) btAfter.Enabled = true;
            pnVisualization.Tag = (int)pnVisualization.Tag - 1;
            if ((int)pnVisualization.Tag == 0) btBefore.Enabled = false;

            if (txtResId.Value != 0)
            {
                txtResId.Value = atoms.ElementAt((int)pnVisualization.Tag).ResSeq;
            }
            updateView(atoms.ElementAt((int)pnVisualization.Tag));
        }

        /// <summary>
        /// Update the view of molecule
        /// </summary>
        /// <param name="atom">The central atom</param>
        private void updateView(Atom atom)
        {
            if (atom == null || !cbView.Checked) return;
            Graphics graphics = pnVisualization.CreateGraphics();
            graphics.Clear(Color.Gray);
            graphics.DrawLine(new Pen(Color.Black), new Point(pnVisualization.Size.Width / 2, 0), new Point(pnVisualization.Size.Width / 2, pnVisualization.Size.Height));
            graphics.DrawLine(new Pen(Color.Black), new Point(0, pnVisualization.Size.Height / 2), new Point(pnVisualization.Size.Width, pnVisualization.Size.Height / 2));
            if(!cbDiscretizableDis.Checked) graphics.DrawEllipse(new Pen(Color.Black), pnVisualization.Size.Width / 2 - (int)tbDis.Value * (int)tbScale.Value, pnVisualization.Size.Height / 2 - (int)tbDis.Value * (int)tbScale.Value, (int)tbDis.Value * 2 * (int)tbScale.Value, (int)tbDis.Value * 2 * (int)tbScale.Value);
            int t;
            if (tbScale.Value < 30) t = 2;
            else t = 3;

            foreach (var a in atoms)
            {
                if (isValidDistance(atom, a) && (txtResId.Value == 0 || a.ResSeq == txtResId.Value))
                {
                    Pen p = null;
                    if (a.Element.Contains("H"))
                    {
                        p = new Pen(Color.White);
                    }
                    else if (a.Element.Contains("C"))
                    {
                        p = new Pen(Color.Black);
                    }
                    else if (a.Element.Contains("N"))
                    {
                        p = new Pen(Color.DarkBlue);
                    }
                    else if (a.Element.Contains("O"))
                    {
                        p = new Pen(Color.Red);
                    }
                    else if (a.Element.Contains("P"))
                    {
                        p = new Pen(Color.Orange);
                    }
                    else
                    {
                        p = new Pen(Color.Coral);
                    }
                    if (cbChainMark.Checked && a.ChainID != 'A')
                    {
                        p = new Pen(Color.HotPink);
                    }
                    graphics.DrawEllipse(p, (int)((a.X - atom.X) * (int)tbScale.Value + pnVisualization.Size.Width / 2) - 1, (int)((int)tbScale.Value * (a.Y - atom.Y) + pnVisualization.Size.Height / 2) - 1, t, t);
                }
            }
            graphics.DrawString(atom.Serial + " " + atom.Name + " " + atom.ChainID + ", " + atom.ResName + "," + atom.ResSeq, new Font(FontFamily.GenericSansSerif, 9), new Pen(Color.White).Brush, 0, 0);
        }

        /// <summary>
        /// Sets the conversion for use the hand-crafted vertex order
        /// </summary>
        private void cbHcOrder_CheckedChanged(object sender, EventArgs e)
        {
            return;
            //if (cbHcOrder.Checked)
            //{
            //    txtFilter.Text = "";
            //    txtFilter.Enabled = false;
            //}
            //else
            //{
            //    txtFilter.Enabled = true;
            //}
            if (pnVisualization.Tag == null) return;
            Atom current = atoms.ElementAt((int)pnVisualization.Tag);
            List<Atom> neighbors = new List<Atom>();
            foreach (var a in atoms)
            {
                if (!current.Equals(a) && isValidDistance(current, a))
                {
                    neighbors.Add(a);
                }
            }


            Atom less = null;
            double lessD = double.MaxValue;

            if (current.Element.Contains("N"))
            {
                double delta = 1.5;
                double error = double.MaxValue;
                Graphics graphics = pnVisualization.CreateGraphics();
                graphics.DrawEllipse(new Pen(Color.White), pnVisualization.Size.Width / 2 - (float)(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2) + delta) * (int)tbScale.Value, pnVisualization.Size.Height / 2 - (float)(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2) + delta) * (int)tbScale.Value, (float)(delta + AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2)) * 2 * (int)tbScale.Value, (float)(delta + AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2)) * 2 * (int)tbScale.Value);
                foreach (var a in neighbors)
                {
                    if (a.Element.Contains("H") && Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2), 2))) < error)
                    {
                        error = Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2), 2)));
                    }
                }
                if (error >= delta) { MessageBox.Show("This atom not belong the backbone h" + (error).ToString()); return; }
                error = double.MaxValue;
                foreach (var a in neighbors)
                {
                    if (a.Element.Contains("C") && Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.CA2), 2))) < error)
                    {
                        error = Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.CA2), 2)));
                    }
                }
                if (error >= delta) { MessageBox.Show("This atom not belong the backbone c" + error.ToString()); return; }

                foreach (var a in neighbors)
                {
                    if (a.Element.Contains("H") && Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2), 2))) < lessD)
                    {
                        lessD = Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2), 2)));
                        less = a;
                    }
                }

            }
            else if (current.Element.Contains("H"))
            {
                foreach (var a in neighbors)
                {
                    if (a.Element.Contains("C") && current.Distance(a) < lessD)
                    {
                        lessD = current.Distance(a);
                        less = a;
                    }
                }
            }
            if (less == null) return;
            pnVisualization.Tag = atoms.FindIndex(new Predicate<Atom>(delegate (Atom a) { return a.Equals(less); }));
            updateView(less);

        }

        /// <summary>
        /// Skip to the near neighbor
        /// </summary>
        private void btNear_Click(object sender, EventArgs e)
        {
            if (pnVisualization.Tag == null) return;
            if ((bool)btNear.Tag == true)
            {
                btNear.Tag = false;
                updateView(atoms.ElementAt((int)pnVisualization.Tag));
                return;
            }
            else btNear.Tag = true;
            Atom current = atoms.ElementAt((int)pnVisualization.Tag);
            List<Atom> neighbors = new List<Atom>();
            foreach (var a in atoms)
            {
                if (!current.Equals(a) && isValidDistance(current, a))
                {
                    neighbors.Add(a);
                }
            }
            Atom less = neighbors.ElementAt(0);
            double lessD = current.Distance(less);
            foreach (var a in neighbors)
            {
                if (current.Distance(a) < lessD)
                {
                    lessD = current.Distance(a);
                    less = a;
                }
            }
            updateView(less);
        }

        private void btHc_Click(object sender, EventArgs e)
        {

            bool flag = false;
            while (!flag)
            {
                if (pnVisualization.Tag == null) return;
                Atom current = atoms.ElementAt((int)pnVisualization.Tag);
                List<Atom> neighbors = new List<Atom>();
                foreach (var a in atoms)
                {
                    if (!current.Equals(a) && isValidDistance(current, a))
                    {
                        neighbors.Add(a);
                    }
                }


                Atom less = null;
                double lessD = double.MaxValue;

                if (current.Element.Contains("N"))
                {
                    double error = double.MaxValue;
                    foreach (var a in neighbors)
                    {
                        if (a.Element.Contains("H") && Math.Sqrt(Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2), 2)) < error)
                        {
                            error = Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2), 2)));
                        }
                    }
                    if (error >= 1.5) { MessageBox.Show("This atom not belong the backbone h" + error.ToString()); goto teste; }
                    error = double.MaxValue;
                    foreach (var a in neighbors)
                    {
                        if (a.Element.Contains("C") && Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.CA2), 2))) < error)
                        {
                            error = Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.CA2), 2)));
                        }
                    }
                    if (error >= 1.5) { MessageBox.Show("This atom not belong the backbone c" + error.ToString()); goto teste; }

                    flag = true;
                    foreach (var a in neighbors)
                    {
                        if (a.Element.Contains("H") && Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2), 2))) < lessD)
                        {
                            lessD = Math.Sqrt((Math.Pow(current.Distance(a), 2) - Math.Pow(AminoAcidAtom.N2.GetDistance(AminoAcidAtom.H2), 2)));
                            less = a;
                        }
                    }
                }
            teste: btAfter_Click(sender, e);
            }
        }

        /// <summary>
        /// Set as current the atom with less distance of the double click
        /// </summary>
        private void pnVisualization_DoubleClick(object sender, EventArgs e)
        {
            if (pnVisualization.Tag == null || atoms.ElementAt((int)pnVisualization.Tag) == null) return;
            Atom amin = null, acur = atoms.ElementAt((int)pnVisualization.Tag),
                atom = new Atom(0,
                (((MouseEventArgs)e).X + 1 - pnVisualization.Size.Width / 2) / (double)tbScale.Value + acur.X,
                (((MouseEventArgs)e).Y + 1 - pnVisualization.Size.Height / 2) / (double)tbScale.Value + acur.Y,
                0);
            double min = double.MaxValue;
            foreach (var a in atoms)
            {
                double cache;
                atom.Z = a.Z;
                if ((cache = atom.Distance(a)) < min)
                {
                    min = cache;
                    amin = a;
                }
            }
            if (amin == null) return;

            pnVisualization.Tag = atoms.IndexOf(amin);
            if ((int)pnVisualization.Tag == 0) btBefore.Enabled = false;
            if ((int)pnVisualization.Tag == atoms.Count - 1) btAfter.Enabled = false;

            updateView(atoms.ElementAt((int)pnVisualization.Tag));
        }

        private void cbChainMark_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (pnVisualization.Tag != null)
                    updateView(atoms.ElementAt((int)pnVisualization.Tag));
            }
            catch (Exception)
            {
            }
        }

        private void txtResId_ValueChanged(object sender, EventArgs e)
        {
            if (!cbInstante.Checked) return;
            try
            {
                if (pnVisualization.Tag != null)
                    updateView(atoms.ElementAt((int)pnVisualization.Tag));
            }
            catch (Exception)
            {
            }
        }

        private void tbScale_ValueChanged(object sender, EventArgs e)
        {
            if (!cbInstante.Checked) return;
            try
            {
                if (pnVisualization.Tag != null)
                    updateView(atoms.ElementAt((int)pnVisualization.Tag));
            }
            catch (Exception)
            {
            }
        }

        private void tbDis_ValueChanged(object sender, EventArgs e)
        {
            if (!cbInstante.Checked) return;
            try
            {
                if (pnVisualization.Tag != null)
                    updateView(atoms.ElementAt((int)pnVisualization.Tag));
            }
            catch (Exception)
            {
            }
        }

        private void pnVisualization_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    updateView(new Atom(0,
                        (e.X + 1 - pnVisualization.Size.Width / 2) / (double)tbScale.Value + atoms.ElementAt((int)pnVisualization.Tag).X,
                        (e.Y + 1 - pnVisualization.Size.Height / 2) / (double)tbScale.Value + atoms.ElementAt((int)pnVisualization.Tag).Y,
                        0));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void pnVisualization_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    updateView(atoms.ElementAt((int)pnVisualization.Tag));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void fMain_SizeChanged(object sender, EventArgs e)
        {
            pnVisualization.Size = new Size(this.Size.Width-pnVisualization.Location.X-23, this.Size.Height - pnVisualization.Location.Y - 48);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void cbOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOutput.SelectedIndex == 0)
            {
                //cbConvertFile.Text = cbConvertFile.Text
            }
            else if (cbOutput.SelectedIndex == 1)
            {

            }
            else if (cbOutput.SelectedIndex == 2)
            {

            }
            else if (cbOutput.SelectedIndex == 3)
            {

            }
            else if (cbOutput.SelectedIndex == 4)
            {

            }
            else if (cbOutput.SelectedIndex == 5)
            {

            }
            else if (cbOutput.SelectedIndex == 6)
            {

            }
        }
        
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (cbInstante.Checked)
            {
                if (txtFilter.Tag != null) atoms = (List<Atom>)txtFilter.Tag;
                if (String.IsNullOrWhiteSpace(txtFilter.Text))
                {
                    updateView(atoms.ElementAt((int)pnVisualization.Tag));
                    txtFilter.Tag = null;
                }
                else
                {
                    List<Atom> validAtoms = new List<Atom>();
                    foreach (var a in atoms)
                    {
                        if (a.Element==txtFilter.Text) validAtoms.Add(a);
                    }
                    txtFilter.Tag = atoms;
                    atoms = validAtoms;
                    if (atoms.Count > 0) updateView(atoms.First<Atom>()); else updateView(null);
                    
                }
            }
        }

        private void cbDiscretizableDis_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDiscretizableDis.Checked)
            {
                lbDis.Text = "Radium (Nr)";
                tbDisAlt.Value = tbDis.Value;
                tbDis.Visible = false;
                tbDisAlt.Visible = true;
            }
            else
            {
                lbDis.Text = "Radium (Å)";
                tbDis.Value = tbDisAlt.Value;
                tbDis.Visible = true;
                tbDisAlt.Visible = false;
            }
        }

        /// <summary>
        /// This functions returns if the distance between the two atoms is valid
        /// </summary>
        /// <param name="a">The first Atom</param>
        /// <param name="b">The secound Atom</param>
        /// <returns>True if the distance is valid or, otherwise, false</returns>
        private bool isValidDistance(Atom a, Atom b)
        {
            return (cbDiscretizableDis.Checked) ? a.DistanceDiscretized(b, atoms) <= tbDisAlt.Value : a.Distance(b) <= (double)tbDis.Value;
        }
    }
}
