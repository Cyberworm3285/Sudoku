using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ExtensionMethods;

namespace Sudoku_Solver
{
    public partial class Form1 : Form
    {
        Sudoku sud;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            sud = new Sudoku(this);
            sud.bruteForce();
        }
    }

    public class Sudoku
    {
        public SudChunk[,] chunks;
        public Feld[,] felder;
        int[,] inputNum;

        public Sudoku(Form1 owner)
        {
            chunks = new SudChunk[3, 3];
            felder = new Feld[9, 9];
            string pathBase = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "input.txt");

            string[] rawInput = File.ReadAllLines(pathBase);
            inputNum = new int[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    inputNum[j, i] = rawInput[i][j].ToInt();
                }
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    chunks[i, j] = new SudChunk(inputNum, new Point(i, j), owner, ref felder);
                }
            }
        }

        public void bruteForce()
        {
            if (!solve(chunks, felder))
            {
                Tuple<int, int> coord = getLeastLeft();
                Feld temp = felder[coord.Item1, coord.Item2];
                temp.zahl = getPossi(temp.x, temp.y,felder)[temp.bruteCount++];
            }
        }

        private bool solve(SudChunk[,] chu, Feld[,] fel)
        {
            int changes = 1;

            while (changes > 0)
            {
                changes = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (fel[i, j].zahl != 0) continue;
                        List<int> left = getPossi(i, j, this.felder);

                        if (left.Count == 1 || fel[i,j].ownerChunk.FieldsLeft() == 1)
                        {
                            if (fel[i, j].ownerChunk.felder.Contains(left.First())) continue;
                            fel[i, j].setZahl(left.First());
                            changes++;
                        }
                    }
                }
            }
            return (this.FieldsLeft() == 0);
        }

        private void applyChanges(Feld[,] fel)
        {
            foreach(Feld f in fel)
            {
               // this.
            }
        }

        private int FieldsLeft()
        {
            int result = 0;
            foreach (Feld f in felder)
            {
                if (f.zahl == 0)
                {
                    result++;
                }
            }
            return result;
        }

        private Tuple<int,int> getLeastLeft()
        {
            Tuple<int,int>  momBest = null;
            int bestC = 0;

            foreach (Feld f in this.felder)
            {
                if (momBest == null || getPossi(f.x,f.y,felder).Count < bestC)
                {
                    momBest = Tuple.Create(f.x, f.y);
                }
            }
            return momBest;
        }

        private List<int> getPossi(int x, int y, Feld[,] fel)
        {
            int[] allNumbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<int> left = allNumbers.ToList<int>();
            //horizontal
            for (int k = 0; k < 9; k++)
            {
                if (left.Contains(fel[x, k].zahl)) left.Remove(fel[x, k].zahl);
            }
            //vertikal
            for (int k = 0; k < 9; k++)
            {
                if (left.Contains(fel[k, y].zahl)) left.Remove(fel[k, y].zahl);
            }
            foreach (Feld f in fel[x,y].ownerChunk.felder)
            {
                if (left.Contains(f.zahl)) left.Remove(fel[x, y].zahl);
            }
            return left;
        }
    }

    public struct SudChunk
    {
        public Feld[,] felder;
        static int size = 25;

        public SudChunk(int[,] a,Point reference, Form1 owner, ref Feld[,] allFeld)
        {
            felder = new Feld[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int num = a[reference.X * 3 + i, reference.Y * 3 + j];
                    Feld temp = new Feld(i,j,num, this);
                    temp.Name = i + "_" + j;
                    temp.Text = (num==0)?"-":num.ToString();
                    temp.Size = new Size(size, size);
                    temp.Location = new Point(reference.X * 3 * size + i * size, reference.Y * 3 * size + j * size);
                    felder[i, j] = temp;
                    allFeld[reference.X * 3 + i, reference.Y * 3 + j] = temp;
                    owner.Controls.Add(temp);
                }
            }
        }

        public int FieldsLeft()
        {
            int result = 0;
            foreach(Feld f in felder)
            {
                if (f.zahl == 0)
                {
                    result++;
                }
            }
            return result;
        }
    }

    public class Feld : Button
    {
        public int zahl;
        public SudChunk ownerChunk;
        public int x, y;
        public int bruteCount = 0;

        public Feld(int a, int b, int c, SudChunk d): base()
        {
            zahl = a;
            ownerChunk = d;
        }

        public void setZahl(int a)
        {
            zahl = a;
            this.BackColor = Color.Red;
            this.Text = a.ToString();
        }
    }
}

namespace ExtensionMethods
{
    public static class Extensions
    {
        public static int ToInt(this char c)
        {
            int i;
            int.TryParse(c.ToString(), out i);
            return i;
        }

        public static bool Contains(this Sudoku_Solver.Feld[,] f, int i)
        {
            bool result = false;
            foreach (Sudoku_Solver.Feld feld in f)
            {
                if (feld.zahl == i)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
