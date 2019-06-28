﻿using PDBReader.classes;
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
            dialog.Filter = "PDB files (*.ent)|*.ent|All files (*.*)|*.*";
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
                        foreach (var aux in atoms) //não repetir models
                        {
                            if (a.Serial == aux.Serial)
                                goto end;
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
                if (cbOutput.SelectedItem == cbOutput.Items[0])
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
                else if (cbOutput.SelectedItem == cbOutput.Items[1])
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
                        int current = 0;
                        for (int i = 0; i < atomsSingle.Count; i++)
                        {
                            if (first) first = false; else file.Append(";");
                            List<Atom> neighbors = new List<Atom>();
                            current = atoms.IndexOf(atomsSingle.ElementAt(i));
                            for (int j = 1; j < 4 && i - j >= 0; j++)
                            {
                                neighbors.Add(atoms.ElementAt(current - j));
                            }
                            foreach (Atom a in atomsSingle)
                            {
                                if (neighbors.Exists(aux => aux == a))
                                    file.Append(" " + atomsSingle.ElementAt(i).Distance(a).ToString(new CultureInfo("en-US")));
                                else file.Append(" 0");
                            }
                        }
                        file.Append("]");
                        File.WriteAllText(cbConvertFile.Text, file.ToString());
                    }
                    else
                    {
                        string file = "PDB = [";
                        bool first = true;
                        foreach (Atom a in atoms)
                        {
                            if (first) first = false; else file += ";";
                            file += a.X.ToString(new CultureInfo("en-US")) + " " + a.Y.ToString(new CultureInfo("en-US")) + " " + a.Z.ToString(new CultureInfo("en-US"));
                        }
                        file += "]";
                        File.WriteAllText(cbConvertFile.Text, file);
                    }
                }
                else if (cbOutput.SelectedItem == cbOutput.Items[2])
                {
                    if (cbBP.Checked)
                    {
                        List<Atom> atomsSingle = new List<Atom>();
                        for (int i = 0; i < atoms.Count; i++)
                        {
                            if (atoms.IndexOf(atoms.ElementAt(i)) < i) continue;
                            atomsSingle.Add(atoms.ElementAt(i));
                        }
                        string file = "{\"BP\":{";
                        file += "V:[";
                        bool first = true;
                        foreach (Atom a in atoms)
                        {
                            if (first) first = false; else file += ",";
                            file += "{\"Vi\":\"" + "v" + atomsSingle.IndexOf(a) + "\"}";
                        }
                        file += "],E:[";
                        first = true;
                        int current = 0;
                        for (int i = 0; i < atomsSingle.Count; i++)
                        {
                            current = atoms.IndexOf(atomsSingle.ElementAt(i));
                            for (int j = 1; j < 4 && i-j>=0; j++)
                            {
                                if (first) first = false; else file += ",";
                                file += "{\"v" + i + ",v" + atomsSingle.IndexOf(atoms.ElementAt(current - j)) +
                                    "\":" + atomsSingle.ElementAt(i).Distance(atoms.ElementAt(current - j)).ToString(new CultureInfo("en-US")) + "}";
                            }
                        }
                        file += "]}}";
                        File.WriteAllText(cbConvertFile.Text, file);
                    }
                    else
                    {
                        string file = "{\"atoms\":[";
                        bool first = true;
                        foreach (Atom a in atoms)
                        {
                            if (first) first = false; else file += ",";
                            file += "{\"name\":\"" + a.Name + "\",\"position\":{\"x\":" + a.X.ToString(new CultureInfo("en-US")) + ",\"y\":" + a.Y.ToString(new CultureInfo("en-US")) + ",\"z\":" + a.Z.ToString(new CultureInfo("en-US")) + "}}";
                        }
                        file += "]}";
                        File.WriteAllText(cbConvertFile.Text, file);
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
                if (atom.Distance(a) < (double)tbDis.Value && (txtResId.Value == 0 || a.ResSeq == txtResId.Value))
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
            graphics.DrawEllipse(new Pen(Color.Black), pnVisualization.Size.Width / 2 - (int)tbDis.Value * (int)tbScale.Value, pnVisualization.Size.Height / 2 - (int)tbDis.Value * (int)tbScale.Value, (int)tbDis.Value * 2 * (int)tbScale.Value, (int)tbDis.Value * 2 * (int)tbScale.Value);
            int t;
            if (tbScale.Value < 30) t = 2;
            else t = 3;

            foreach (var a in atoms)
            {
                if (atom.Distance(a) < (double)tbDis.Value && (txtResId.Value == 0 || a.ResSeq == txtResId.Value))
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
                if (!current.Equals(a) && current.Distance(a) < (double)tbDis.Value)
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
        /// Skip to the more near neighbor
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
                if (!current.Equals(a) && current.Distance(a) < (double)tbDis.Value)
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
                    if (!current.Equals(a) && current.Distance(a) < (double)tbDis.Value)
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
    }
}
