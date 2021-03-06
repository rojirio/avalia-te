﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace avaliate
{
    public partial class MontagemProva : Form
    {
        Questao questao;

        string connString = @"Host=127.0.0.1;Username=postgres;Password=ifsp;Database=postgres";

        int index = 0;

        public MontagemProva()
        {
            InitializeComponent();
            //string[] items = new string[] {"QUESTÃO 1","QUESTÃO 2","QUESTÃO 3","QUESTÃO 4" };
            //listBox1.Items.AddRange(items);

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, titulo FROM questoes WHERE id IN(" +
                    "SELECT fk_questao FROM questao_materia_nivelensino WHERE fk_materia = @materia AND fk_nivelensino = @nivelensino)", conn))
                {
                    cmd.Parameters.AddWithValue("materia", LoginInfo.materia);
                    cmd.Parameters.AddWithValue("nivelensino", LoginInfo.nivelEnsino);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        listBox1.DisplayMember = "titulo";
                        listBox2.DisplayMember = "titulo";
                        listBox1.ValueMember = "id";
                        listBox2.ValueMember = "id";

                        while (reader.Read())
                        {
                            listBox1.Items.Add(new Questao { id = reader.GetInt32(0).ToString(), titulo = reader.GetString(1) });
                    
                        }
                    }
                }
            }


            }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ListBox list1 = sender as ListBox;
            index = list1.IndexFromPoint(e.X, e.Y);

            if (index >= 0 & e.Button == MouseButtons.Left) {

                list1.DoDragDrop(list1.Items[index], DragDropEffects.Move);

            }

        }

        private void listBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Questao))) {

                e.Effect = DragDropEffects.Move;

            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBox2_DragDrop(object sender, DragEventArgs e)
        {
            ListBox list2 = sender as ListBox;
            list2.Items.Add(e.Data.GetData(typeof(Questao)));
            listBox1.Items.RemoveAt(index);
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Questao)))
            {

                e.Effect = DragDropEffects.Move;

            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            ListBox list1 = sender as ListBox;
            list1.Items.Add(e.Data.GetData(typeof(Questao)));
            listBox2.Items.RemoveAt(index);
        }

        private void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
            ListBox list2 = sender as ListBox;
            index = list2.IndexFromPoint(e.X, e.Y);

            if (index >= 0 & e.Button == MouseButtons.Left)
            {

                list2.DoDragDrop(list2.Items[index], DragDropEffects.Move);

            }
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int i=0;
            var fonte =  new Font("Times New Roman", 14);
           

            //           Bitmap bm = new Bitmap(2480, 800);
            //          pictureBox1.DrawToBitmap(bm, new Rectangle(100, 0, pictureBox1.Width, pictureBox1.Height
            e.Graphics.DrawImage(pictureBox1.Image, 0, 0, 825, 450 );
            //              bm.Dispose();

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                foreach (Questao item in listBox2.Items)
                {

                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT enunciado FROM questoes WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", Convert.ToInt32(listBox2.Items[i].ToString()));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string s = Convert.ToString(i + 1) + "- " + reader.GetString(0) + "\n\n";

                                e.Graphics.DrawString(s, fonte, Brushes.Black, new PointF(100, i * 50 + pictureBox1.Image.Height));
                            }

                        }
                    }
                    i++;
                }
            }
            
        }

        private void imprimir_Click(object sender, EventArgs e)
        {
            if (printPreviewDialog.ShowDialog() == DialogResult.OK)
                printDocument.Print();
        }

        private void header_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter =  "PNG|*png" , ValidateNames = true 
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    pictureBox1.Image = Image.FromFile(ofd.FileName);
            }
        }

        public class Questao
        {
            public string titulo { get; set; }
            public string id { get; set; }

            public override string ToString()
            {
                return this.id;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            var menu = new Menu();
            menu.Closed += (s, args) => this.Close();
            menu.Show();
        }
    }
}
