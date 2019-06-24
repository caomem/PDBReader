using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDBReader.classes
{
    /// <summary>
    /// Class Atom represents a atom in a polymer
    /// </summary>
    [Serializable]
    public class Atom
    {
        #region Components

        private int serial;
        /// <summary>
        /// Get or set atom serial number 
        /// </summary>
        public int Serial { get => serial; set => serial = value; }

        private string name;
        /// <summary>
        /// Get or set Atom name
        /// </summary>
        public string Name { get => name; set => name = value; }

        private char altLoc;
        /// <summary>
        /// Get or set Alternate location indicator
        /// </summary>
        public char AltLoc { get => altLoc; set => altLoc = value; }

        private string resName;
        /// <summary>
        /// Get or set Residue name
        /// </summary>
        public string ResName { get => resName; set => resName = value; }

        private char chainID;
        /// <summary>
        /// Get or set Chain identifier
        /// </summary>
        public char ChainID { get => chainID; set => chainID = value; }

        private int resSeq;
        /// <summary>
        /// Get or set Residue sequence number
        /// </summary>
        public int ResSeq { get => resSeq; set => resSeq = value; }

        private char iCode;
        /// <summary>
        /// Get or set Code for insertion of residues
        /// </summary>
        public char ICode { get => iCode; set => iCode = value; }

        private double x;
        /// <summary>
        /// Get or set Orthogonal coordinates for X in Angstroms
        /// </summary>
        public double X { get => x; set => x = value; }

        private double y;
        /// <summary>
        /// Get or set Orthogonal coordinates for Y in Angstroms
        /// </summary>
        public double Y { get => y; set => y = value; }

        private double z;
        /// <summary>
        /// Get or set Orthogonal coordinates for Z in Angstroms
        /// </summary>
        public double Z { get => z; set => z = value; }

        private double occupancy;
        /// <summary>
        /// Get or set Occupancy
        /// </summary>
        public double Occupancy { get => occupancy; set => occupancy = value; }

        private double tempFactor;
        /// <summary>
        /// Get or set Temperature factor
        /// </summary>
        public double TempFactor { get => tempFactor; set => tempFactor = value; }

        private string element;
        /// <summary>
        /// Get or set Element symbol, right-justified
        /// </summary>
        public string Element { get => element; set => element = value; }

        private string charge;
        /// <summary>
        /// Get or set Charge on the atom
        /// </summary>
        public string Charge { get => charge; set => charge = value; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor default for atom
        /// </summary>
        public Atom() { }
        /// <summary>
        /// Constructor for atom with indentification and coordenates
        /// </summary>
        /// <param name="serial">Atom serial number</param>
        /// <param name="x">Orthogonal coordinates for X in Angstroms</param>
        /// <param name="y">Orthogonal coordinates for Y in Angstroms</param>
        /// <param name="z">Orthogonal coordinates for Z in Angstroms</param>
        public Atom(int serial, double x, double y, double z)
        {
            Serial = serial;
            X = x;
            Y = y;
            Z = z;
        }
        /// <summary>
        /// Constructor for atom with indentification
        /// </summary>
        /// <param name="name">Name of atom</param>
        /// <param name="element">Element of atom</param>
        /// <param name="resName">Residue name of atomo</param>
        public Atom(string name, string element, string resName)
        {
            Name = name;
            Element = element;
            ResName = resName;
        }

        #endregion

        /// <summary>
        /// Convert the atom instance in string
        /// </summary>
        /// <returns>Retorns the serial, element simbol, coordnates X, Y and Z as string description of atom</returns>
        public override string ToString()
        {
            return "serial = " + Serial.ToString() + " name: "+Name+ " chain: "+ChainID+" res: {"+ResName+", "+ResSeq+"} x = " + X.ToString()
                + " y = " + Y.ToString() + " z = " + Z.ToString()+ " " + Element;
        }

        /// <summary>
        /// Return the distance between this instance and the atom in parameter
        /// </summary>
        /// <param name="atom">Atom for calculate the distance</param>
        /// <returns>Distance in angstroms between this atoms</returns>
        public double Distance(Atom atom)
        {
            if (atom == null) return -1;
            return Math.Sqrt(Math.Pow(X - atom.X, 2) + Math.Pow(Y - atom.Y, 2) + Math.Pow(Z - atom.Z, 2));
        }

        /// <summary>
        /// Return the AminoAcidName equivalent
        /// </summary>
        /// <returns>The equivalente AminoAcidName</returns>
        public AminoAcidName GetAminoAcidName()
        {
            if (ResName == null || string.IsNullOrWhiteSpace(ResName)) throw new Exception("Error in Atom.AminoAcidName(): ResName null");
            if (ResName == "ALA") return AminoAcidName.Alanine;
            else if (ResName == "ARG") return AminoAcidName.Arginine;
            else if (ResName == "HIS") return AminoAcidName.Histidine;
            else if (ResName == "LYS") return AminoAcidName.Lysine;
            else if (ResName == "ASP") return AminoAcidName.AsparticAcid;
            else if (ResName == "GLU") return AminoAcidName.GlutamicAcid;
            else if (ResName == "SER") return AminoAcidName.Serine;
            else if (ResName == "THR") return AminoAcidName.Threonine;
            else if (ResName == "ASN") return AminoAcidName.Asparagine;
            else if (ResName == "GLN") return AminoAcidName.Glutamine;
            else if (ResName == "CYS") return AminoAcidName.Cysteine;
            else if (ResName == "SEC") return AminoAcidName.Selenocysteine;
            else if (ResName == "GLY") return AminoAcidName.Glycine;
            else if (ResName == "PRO") return AminoAcidName.Proline;
            else if (ResName == "VAL") return AminoAcidName.Valine;
            else if (ResName == "ILE") return AminoAcidName.Isoleucine;
            else if (ResName == "LEU") return AminoAcidName.Leucine;
            else if (ResName == "MET") return AminoAcidName.Methionine;
            else if (ResName == "PHE") return AminoAcidName.Phenylalanine;
            else if (ResName == "TYR") return AminoAcidName.Tyrosine;
            else if (ResName == "TRP") return AminoAcidName.Tryptophan;
            else throw new Exception("Error in Atom.AminoAcidName(): ResName invalid");
        }

        /// <summary>
        /// Return the AminoAcidAtom equivalent
        /// </summary>
        /// <returns>The equivalente AminoAcidAtom</returns>
        public AminoAcidAtom GetAminoAcidAtom()
        {
            if (Name == null || string.IsNullOrWhiteSpace(Name)) throw new Exception("Error in Atom.AminoAcidAtom(): Name null");
            switch (GetAminoAcidName())
            {
                case AminoAcidName.Alanine:
                    if (Name == "N") return AminoAcidAtom.N;
                    else if (Name == "CA") return AminoAcidAtom.CA;
                    else if (Name == "C") return AminoAcidAtom.C;
                    else if (Name == "O") return AminoAcidAtom.O;
                    else if (Name == "CB") return AminoAcidAtom.CB;
                    else if (Name == "OXT") return AminoAcidAtom.OXT;
                    else if (Name == "H") return AminoAcidAtom.H;
                    else if (Name == "H2") return AminoAcidAtom.H2;
                    else if (Name == "HA") return AminoAcidAtom.HA;
                    else if (Name == "HB1") return AminoAcidAtom.HB1;
                    else if (Name == "HB2") return AminoAcidAtom.HB2;
                    else if (Name == "HB3") return AminoAcidAtom.HB3;
                    else if (Name == "HXT") return AminoAcidAtom.HXT;
                    else throw new Exception("Error in Atom.AminoAcidAtom(): Invalid Name ("+Name+") for "+ResName+" amino");
                case AminoAcidName.Arginine:
                    break;
                case AminoAcidName.Asparagine:
                    break;
                case AminoAcidName.AsparticAcid:
                    break;
                case AminoAcidName.Cysteine:
                    break;
                case AminoAcidName.Glutamine:
                    break;
                case AminoAcidName.GlutamicAcid:
                    break;
                case AminoAcidName.Glycine:
                    break;
                case AminoAcidName.Histidine:
                    break;
                case AminoAcidName.Isoleucine:
                    break;
                case AminoAcidName.Leucine:
                    break;
                case AminoAcidName.Lysine:
                    break;
                case AminoAcidName.Methionine:
                    break;
                case AminoAcidName.Phenylalanine:
                    break;
                case AminoAcidName.Proline:
                    break;
                case AminoAcidName.Serine:
                    break;
                case AminoAcidName.Threonine:
                    break;
                case AminoAcidName.Tryptophan:
                    break;
                case AminoAcidName.Tyrosine:
                    break;
                case AminoAcidName.Valine:
                    break;
                case AminoAcidName.Selenocysteine:
                    break;
                case AminoAcidName.Pyrrolysine:
                    break;
                default:
                    throw new Exception("Error in Atom.AminoAcidAtom(): Invalid AminoAcidName");
            }

            return AminoAcidAtom.C;
        }

        public double TeoricalDistance(Atom atom)
        {
            switch (GetAminoAcidName())
            {
                case AminoAcidName.Alanine:
                    switch (GetAminoAcidAtom())
                    {
                        case AminoAcidAtom.CA:
                            switch (atom.GetAminoAcidAtom())
                            {
                                case AminoAcidAtom.CA:
                                    break;
                                case AminoAcidAtom.C:
                                    break;
                                case AminoAcidAtom.H2:
                                    break;
                                case AminoAcidAtom.HA:
                                    break;
                                case AminoAcidAtom.O:
                                    break;
                                case AminoAcidAtom.N:
                                    break;
                                case AminoAcidAtom.CB:
                                    break;
                                case AminoAcidAtom.OXT:
                                    break;
                                case AminoAcidAtom.H:
                                    break;
                                case AminoAcidAtom.HB1:
                                    break;
                                case AminoAcidAtom.HB2:
                                    break;
                                case AminoAcidAtom.HB3:
                                    break;
                                case AminoAcidAtom.HXT:
                                    break;
                                default:
                                    throw new Exception("Error in Atom.TeoricalDistance(): Invalid AminoAcidAtom of param");
                            }
                            break;
                        case AminoAcidAtom.C:
                            break;
                        case AminoAcidAtom.H2:
                            break;
                        case AminoAcidAtom.HA:
                            break;
                        case AminoAcidAtom.O:
                            break;
                        case AminoAcidAtom.N:
                            break;
                        case AminoAcidAtom.CB:
                            break;
                        case AminoAcidAtom.OXT:
                            break;
                        case AminoAcidAtom.H:
                            break;
                        case AminoAcidAtom.HB1:
                            break;
                        case AminoAcidAtom.HB2:
                            break;
                        case AminoAcidAtom.HB3:
                            break;
                        case AminoAcidAtom.HXT:
                            break;
                        default:
                            throw new Exception("Error in Atom.TeoricalDistance(): Invalid AminoAcidAtom");
                    }
                    break;
                case AminoAcidName.Arginine:
                    break;
                case AminoAcidName.Asparagine:
                    break;
                case AminoAcidName.AsparticAcid:
                    break;
                case AminoAcidName.Cysteine:
                    break;
                case AminoAcidName.Glutamine:
                    break;
                case AminoAcidName.GlutamicAcid:
                    break;
                case AminoAcidName.Glycine:
                    break;
                case AminoAcidName.Histidine:
                    break;
                case AminoAcidName.Isoleucine:
                    break;
                case AminoAcidName.Leucine:
                    break;
                case AminoAcidName.Lysine:
                    break;
                case AminoAcidName.Methionine:
                    break;
                case AminoAcidName.Phenylalanine:
                    break;
                case AminoAcidName.Proline:
                    break;
                case AminoAcidName.Serine:
                    break;
                case AminoAcidName.Threonine:
                    break;
                case AminoAcidName.Tryptophan:
                    break;
                case AminoAcidName.Tyrosine:
                    break;
                case AminoAcidName.Valine:
                    break;
                case AminoAcidName.Selenocysteine:
                    break;
                case AminoAcidName.Pyrrolysine:
                    break;
                default:
                    throw new Exception("Error in Atom.TeoricalDistance(): Invalid AminoAcidName");
            }
            return 0;
        }
    }
}
