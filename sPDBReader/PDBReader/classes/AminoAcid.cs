using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDBReader.classes
{
    /// <summary>
    /// Class to represent an Amino Acid
    /// </summary>
    public class AminoAcid
    {
        #region Components

        private AminoAcidName name;
        /// <summary>
        /// The name of Amino Acid
        /// </summary>
        public AminoAcidName Name { get => name; set => name = value; }


        private readonly List<Atom> backbone;
        /// <summary>
        /// The main protein's chain 
        /// </summary>
        public List<Atom> Backbone { get => backbone; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor to amino acid
        /// </summary>
        /// <param name="name">The amino acid identification</param>
        public AminoAcid(AminoAcidName name)
        {
            Name = name;
            switch (name)
            {
                case AminoAcidName.Alanine:
                    backbone = new List<Atom>();
                    backbone.Add(new Atom("N", "N", "ALA"));
                    backbone.Add(new Atom("CA", "C", "ALA"));
                    backbone.Add(new Atom("C", "C", "ALA"));
                    backbone.Add(new Atom("O", "O", "ALA"));
                    backbone.Add(new Atom("CB", "C", "ALA"));
                    backbone.Add(new Atom("OXT", "O", "ALA"));
                    backbone.Add(new Atom("H", "H", "ALA"));
                    backbone.Add(new Atom("H2", "H", "ALA"));
                    backbone.Add(new Atom("HA", "H", "ALA"));
                    backbone.Add(new Atom("HB1", "H", "ALA"));
                    backbone.Add(new Atom("HB2", "H", "ALA"));
                    backbone.Add(new Atom("HB3", "H", "ALA"));
                    backbone.Add(new Atom("HXT", "H", "ALA"));
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
                    throw new Exception("Error to create a amino acid: AminoAcidName not valid");
            }
        }

        #endregion
        
    }

    /// <summary>
    /// The 21 amino acids
    /// </summary>
    public enum AminoAcidName : long
    {
        Alanine = 52<< AminoAcidAtom.C|52 << AminoAcidAtom.CA, Arginine, Asparagine, AsparticAcid, Cysteine, Glutamine, GlutamicAcid, Glycine, Histidine, Isoleucine, Leucine,
        Lysine, Methionine, Phenylalanine, Proline, Serine, Threonine, Tryptophan, Tyrosine, Valine, Selenocysteine, Pyrrolysine
    }

    /// <summary>
    /// PDB Atoms types 
    /// </summary>
    public enum AminoAcidAtom
    {
        CA = 0, N2, C = 6, H2, HA, O, CA2,
        N,
        CB,
        OXT,
        H,
        HB1,
        HB2,
        HB3,
        HXT
    }

    static class AminoAcidAtomMethods
    {
        /// <summary>
        /// Get the distances between two atoms
        /// </summary>
        /// <param name="amino">the atom to compare</param>
        /// <returns>return the vector distances between the atoms</returns>
        public static double GetDistance(this AminoAcidAtom atom, AminoAcidAtom amino)
        {
            
            switch (atom)
            {
                case AminoAcidAtom.N2:
                    goto N2;
                case AminoAcidAtom.CA:
                    goto Ca;
                case AminoAcidAtom.C:
                    goto C;
                case AminoAcidAtom.H2:
                    goto H2;
                case AminoAcidAtom.HA:
                    goto Ha;
                case AminoAcidAtom.O:
                    goto O;
                case AminoAcidAtom.CA2:
                    goto Ca2;
                default:
                    return -1;
            }
        N2: switch (amino)
            {
                case AminoAcidAtom.N2:
                    return 0;
                case AminoAcidAtom.CA2:
                    return 1.45;
                case AminoAcidAtom.CA:
                    return 2.414;
                case AminoAcidAtom.C:
                    return 1.33;
                case AminoAcidAtom.H2:
                    return 0.86;
                case AminoAcidAtom.O:
                    return 2.252;
                default:
                    break;
            }
        Ca: switch (amino)
            {
                case AminoAcidAtom.N2:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.CA:
                    return 0;
                case AminoAcidAtom.C:
                    return 1.52;
                case AminoAcidAtom.H2:
                    return 3.04;
                case AminoAcidAtom.O:
                    return 2.399;
                default:
                    break;
            }
        C: switch (amino)
            {
                case AminoAcidAtom.N2:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.CA:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.C:
                    return 0;
                case AminoAcidAtom.CA2:
                    return 2.431;
                case AminoAcidAtom.H2:
                    return 1.907;
                case AminoAcidAtom.O:
                    return 1.23;
                default:
                    break;
            }
        H2: switch (amino)
            {
                case AminoAcidAtom.N2:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.CA:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.C:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.CA2:
                    return 2.005;
                case AminoAcidAtom.H2:
                    return 0;
                default:
                    break;
            }
        Ha: switch (amino)
            {
                case AminoAcidAtom.N2:
                    break;
                case AminoAcidAtom.CA:
                    break;
                case AminoAcidAtom.C:
                    break;
                case AminoAcidAtom.H2:
                    break;
                case AminoAcidAtom.HA:
                    return 0;
                case AminoAcidAtom.O:
                    break;
                default:
                    break;
            }
        O: switch (amino)
            {
                case AminoAcidAtom.N2:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.CA:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.C:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.O:
                    return 0;
                default:
                    break;
            }
        Ca2: switch (amino)
            {
                case AminoAcidAtom.N2:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.C:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.H2:
                    return amino.GetDistance(atom);
                case AminoAcidAtom.O:
                    return 0;
                default:
                    break;
            }
            return -1;
        }

        /// <summary>
        /// Get the angle between tree atoms
        /// </summary>
        /// <param name="a">first atom</param>
        /// <param name="a2">middle atom</param>
        /// <param name="a3">last atom</param>
        /// <returns>return the angle in middle atom</returns>
        public static double GetAngle(this AminoAcidAtom a, AminoAcidAtom a2, AminoAcidAtom a3)
        {
            if(a2 == AminoAcidAtom.C)
            {
                if(a == AminoAcidAtom.CA)
                {
                    if (a3 == AminoAcidAtom.O) return 121.1;
                    else if (a3 == AminoAcidAtom.N2) return 115.6;
                    return 0;
                }
                else if (a == AminoAcidAtom.O)
                {
                    if (a3 == AminoAcidAtom.N2) return 123.2;
                    else if (a3 == AminoAcidAtom.CA) return 121.1;
                    return 0;
                }
                else if (a == AminoAcidAtom.N2)
                {
                    if (a3 == AminoAcidAtom.O) return 123.2;
                    else if (a3 == AminoAcidAtom.CA) return 115.6;
                    return 0;
                }
            }
            else if (a2 == AminoAcidAtom.N2)
            {
                if (a == AminoAcidAtom.H2)
                {
                    if (a3 == AminoAcidAtom.C) return 119.5;
                    else if (a3 == AminoAcidAtom.CA2) return 118.2;
                    return 0;
                }
                else if (a == AminoAcidAtom.C)
                {
                    if (a3 == AminoAcidAtom.H2) return 119.7;
                    else if (a3 == AminoAcidAtom.CA2) return 121.9;
                    return 0;
                }
                else if (a == AminoAcidAtom.CA2)
                {
                    if (a3 == AminoAcidAtom.H2) return 118.2;
                    else if (a3 == AminoAcidAtom.C) return 121.9;
                    return 0;
                }
            }
            return 0;
        }

        /// <summary>
        /// Get de equivalent nomenclature standardized for wwPDB
        /// </summary>
        /// <returns>returns the name of atom in 3-letters standar of PDB file</returns>
        public static string GetPDBEquivalent(this AminoAcidAtom atom)
        {
            switch (atom)
            {
                case AminoAcidAtom.CA:
                    return "CA";
                case AminoAcidAtom.N2:
                    return "N2";
                case AminoAcidAtom.C:
                    return "C";
                case AminoAcidAtom.H2:
                    return "H2";
                case AminoAcidAtom.HA:
                    return "HA";
                case AminoAcidAtom.O:
                    return "O";
                case AminoAcidAtom.CA2:
                    return "CA2"; 
                case AminoAcidAtom.N:
                    return "N";
                case AminoAcidAtom.CB:
                    return "CB";
                case AminoAcidAtom.OXT:
                    return "OXT";
                case AminoAcidAtom.H:
                    return "H";
                case AminoAcidAtom.HB1:
                    return "HB1";
                case AminoAcidAtom.HB2:
                    return "HB2";
                case AminoAcidAtom.HB3:
                    return "HB3";
                case AminoAcidAtom.HXT:
                    return "HXT";
                default:
                    return null;
            }
        }
    }

}
