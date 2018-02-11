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

namespace TPR1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        int n; //кол-во состояний
        int k; //кол-во стратегий
        int iter; //кол-во итераций
        static int max = 6;
        Bitmap bm;
        
        double[,,] p = new double[max, max, max];
        double[,,] r = new double[max, max, max];

        double[,] q = new double[max, max];

        double[,] v = new double[max, max+1];
        int[,] d = new int[max, max+1];

        private void init()
        {
            n = Convert.ToInt32(textBox1.Text);
            k = Convert.ToInt32(textBox2.Text);
            iter = Convert.ToInt32(textBox3.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            showMatrix();
        }

        void fillMatrix()
        {
            for (int i = 0; i < n; i++)
                for (int j =0; j< n; j++)
                    for (int l = 0; l < k; l++)
                    p[i, j, l] = Convert.ToDouble(dataGridView1.Rows[(i * k)+l].Cells[j + 2].Value);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int l = 0; l < k; l++)
                        r[i, j, l] = Convert.ToDouble(dataGridView1.Rows[(i * k) + l].Cells[j + n + 2].Value);
        }

        void showMatrix()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            init();

            dataGridView1.Columns.Add("N", "Состояния");
            dataGridView1.Columns.Add("K", "Стратегии");
            for (int i = 0; i < n; i++)
            {
                dataGridView1.Columns.Add((i + 1).ToString(), "Вероятность состояния " + (i + 1).ToString());
            }
            for (int i = 0; i < n; i++)
            {
                dataGridView1.Columns.Add((i + 1).ToString(), "Доходность состояния " + (i + 1).ToString());
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < k; j++)
                    dataGridView1.Rows.Add((i + 1).ToString());
            }
            for (int i = 0; i < n * k; i++)
                dataGridView1.Rows[i].Cells[1].Value = ((i % k + 1).ToString());

            dataGridView1.Columns.Add("Dohod", "Ожидаемые доходности");
        }

        void fillTotals()
        {
            for (int i = 0; i < n; i++)
            {
                v[i, 0] = 0;
                d[i, 0] = -1;
            }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < k; j++)
                {
                    q[i, j] = 0;
                    for (int l = 0; l < n; l++)
                        q[i, j] += p[i, l, j] * r[i, l, j];
                }
            for (int i = 1; i <= iter; i++)
            {
                double[] vi = new double[n];
                int[] di = new int[n];
                for (int j = 0; j < n; j++)
                {
                    vi[j] = 0;
                    di[j] = -1;
                    for (int k1 = 0; k1 < k; k1++)
                    {
                        double tmp = q[j, k1];
                        for (int n1 = 0; n1 < n; n1++)
                            tmp += p[j, n1, k1] * v[n1, i - 1];
                        if (vi[j] < tmp)
                        {
                            vi[j] = tmp;
                            di[j] = k1;
                        }
                    }
                    v[j, i] = vi[j];
                    d[j, i] = di[j];
                }
            }
        }

        void showTotals()
        {
            fillTotals();

            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView1.Columns.Remove("Dohod");
            //init();

            dataGridView1.Columns.Add("Dohod", "Ожидаемые доходности");
            dataGridView2.Columns.Add("N", "n");
            for (int i = 0; i < iter + 1; i++)
            {
                dataGridView2.Columns.Add((i + 1).ToString(), "Эпоха " + i.ToString());
            }
            for (int i = 0; i < n; i++)
            {
                dataGridView2.Rows.Add("V" + (i + 1).ToString());
            }
            for (int i = 0; i < n; i++)
            {
                dataGridView2.Rows.Add("D" + (i + 1).ToString());
            }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < iter + 1; j++)
                    dataGridView2.Rows[i].Cells[j + 1].Value = Math.Round(v[i, j], 2).ToString();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < iter + 1; j++)
                    dataGridView2.Rows[i + n].Cells[j + 1].Value = (d[i, j] + 1).ToString();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < k; j++)
                    dataGridView1.Rows[j + i * k].Cells[n * 2 + 2].Value = Math.Round(q[i, j], 2).ToString();
        }

        void showPRMatrix()
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int l = 0; l < k; l++)
                        dataGridView1.Rows[(i * k) + l].Cells[j + 2].Value = p[i, j, l];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int l = 0; l < k; l++)
                        dataGridView1.Rows[(i * k) + l].Cells[j + n + 2].Value = r[i, j, l];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fillMatrix();
            showMatrix();
        }

        private void Drawing()
        {
            Graphics g = Graphics.FromImage(bm);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            /*g.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.TextRenderingHint = */
            //g.Clear(pictureBox1.BackColor);

            Font font = new Font("Arial", 8);

            Pen arrowpen = new Pen(Brushes.LightSlateGray);
            arrowpen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            g.FillRectangle(Brushes.White, 0, 0, pictureBox1.Width, pictureBox1.Height);

            for (int i = 0; i<n;i++)
                for (int j=0; j<n; j++)
                    for(int l=0; l<k; l++)
                    {
                        if (p[i, j, l] != 0)
                        {
                            g.DrawLine(arrowpen, 10*2, 100 * i + 37, 550+10, 100 * j + 37);
                            //g.RotateTransform(10 * (i- j));
                            //g.DrawString((i + 1).ToString(), font, Brushes.Black, 50+l*20, 100 * i + 27);
                        }
                    }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    for (int l = 0; l < k; l++)
                    {
                        if (p[i, j, l] != 0)
                        {
                        //g.DrawLine(arrowpen, 10 * 2, 100 * i + 37, 550 + 10, 100 * j + 37);
                        //g.RotateTransform(10 * (i- j));
                        g.DrawString(((l + 1) + ": " + p[i,j,l] + "; " + r[i,j,l]).ToString(), font, Brushes.Black, 100 + 80 * l, 100 * i + 30 + (j - i) * 15/*(20-(j-i)*10) + 100 * j- 100 * i +20*l+5*(j-i)  100 * i + 27 + (j-i)*11+ l*5   j*10 + (l-j)*20  + 100*j-100*i*/);
                            if (p[i, j, l] != 0)
                                g.DrawString(((i + 1) + " -> " + (j + 1) + ": ").ToString(), font, Brushes.Black, 60, 100 * i + 30 + (j - i) * 15);
                        }
                    }
                    
                }

            for (int i = 0; i < n; i++)
            {
                g.FillEllipse(Brushes.Aqua, 10, 100 * i + 20, 30, 30);
                g.DrawEllipse(arrowpen, 10, 100 * i + 20, 30, 30);
                g.DrawString((i + 1).ToString(), font, Brushes.Black, 10*2, 100 * i + 27);
            }
            for (int i = 0; i < n; i++)
            {
                g.FillEllipse(Brushes.Aqua, 550, 100 * i + 20, 30, 30);
                g.DrawEllipse(arrowpen, 550, 100 * i + 20, 30, 30);
                g.DrawString((i + 1).ToString(), font, Brushes.Black, 550+10, 100 * i + 27);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(bm, 0, 0);
        }

        public void Save(StreamWriter stream)
        {
            stream.WriteLine(n);
            stream.WriteLine(k);
            stream.WriteLine(iter);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int l = 0; l < k; l++)
                    {
                        stream.WriteLine(p[i, j, l].ToString() + " ");
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int l = 0; l < k; l++)
                    {
                        stream.WriteLine(r[i, j, l].ToString() + " ");
                    }
                }
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Файлы для марковских процессов|*.tpr";
            saveFileDialog1.DefaultExt = "tpr";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter stream = new StreamWriter(saveFileDialog1.FileName);
                Save(stream);
                stream.Close();
            }
        }

        public void LoadFile(StreamReader Stream)
        {
            int n = Convert.ToInt32(Stream.ReadLine());
            textBox1.Text = n.ToString();
            int k = Convert.ToInt32(Stream.ReadLine());
            textBox2.Text = k.ToString();
            int iter = Convert.ToInt32(Stream.ReadLine());
            textBox3.Text = iter.ToString();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int l = 0; l < k; l++)
                    {
                        p[i,j,l] = Convert.ToInt32(Stream.ReadLine());
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int l = 0; l < k; l++)
                    {
                        r[i, j, l] = Convert.ToInt32(Stream.ReadLine());
                    }
                }
            }

            showMatrix();
            showPRMatrix();
            showTotals();
            pictureBox1.Image = null;
            Drawing();
            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Файлы для марковских процессов|*.tpr";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader stream = new StreamReader(openFileDialog1.FileName);
                LoadFile(stream);
                stream.Close();
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;

            Drawing();

            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
        }
    }
}