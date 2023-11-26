using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace pkglab4
{
    public partial class Form1 : Form
    {
        int scale = 14;
        int shift = 7;
        bool circle = false;
        public Form1()
        {
            InitializeComponent();
            rasterizer = new Rasterizer();
            bruh = new Pen(Color.FromArgb(80, 80, 80), 4);
            start = new Point(0, 0);
            end = new Point(0, 0);
            center = new Point(0, 0);
            radius = 0;
        }
        private System.Diagnostics.Stopwatch watch;
        private Point start, end, center;
        private int radius;
        Pen bruh;
        Rasterizer rasterizer;
        public void drawMarkup(Graphics gr, Panel panel, VScrollBar vsb, HScrollBar hsb)
        {
            Pen pen_digits = new Pen(Color.Red, 1);
            var cx = panel.Width / 2 + (hsb.Value - hsb.Maximum / 2) * shift;
            var cy = panel.Height / 2 - (vsb.Value - vsb.Maximum / 2) * shift;
            int cur_x = 0, cur_y = 0;

            Font font = new Font("Arial", shift - 1);
            int counter = 0;
            gr.DrawString(counter.ToString(), font, pen_digits.Brush, new PointF(cx, cy));
            if (cx <= panel.Width / 2)
            {
                while (cx + cur_x <= panel.Width)
                {
                    counter++;
                    cur_x += scale;
                    PointF pr = new PointF(cx + cur_x, cy);
                    PointF pl = new PointF(cx - cur_x, cy);
                    gr.DrawString(counter.ToString(), font, pen_digits.Brush, pr);
                    gr.DrawString("-" + counter.ToString(), font, pen_digits.Brush, pl);
                }
            }
            else
            {
                while (cx - cur_x >= 0)
                {
                    counter++;
                    cur_x += scale;
                    PointF pr = new PointF(cx + cur_x, cy);
                    PointF pl = new PointF(cx - cur_x, cy);
                    gr.DrawString(counter.ToString(), font, pen_digits.Brush, pr);
                    gr.DrawString("-" + counter.ToString(), font, pen_digits.Brush, pl);
                }
            }
            counter = 0;
            if (cy <= panel.Height / 2)
            {
                while (cy + cur_y <= panel.Height)
                {
                    counter++;
                    cur_y += scale;
                    PointF pl = new PointF(cx, cy + cur_y);
                    PointF pr = new PointF(cx, cy - cur_y);
                    gr.DrawString(counter.ToString(), font, pen_digits.Brush, pr);
                    gr.DrawString("-" + counter.ToString(), font, pen_digits.Brush, pl);

                }
            }
            else
            {
                while (cy - cur_y >= 0)
                {
                    counter++;
                    cur_y += scale;
                    PointF pl = new PointF(cx, cy + cur_y);
                    PointF pr = new PointF(cx, cy - cur_y);
                    gr.DrawString(counter.ToString(), font, pen_digits.Brush, pr);
                    gr.DrawString("-" + counter.ToString(), font, pen_digits.Brush, pl);

                }
            }
            while (cx - cur_x >= 0)
            {
                counter++;
                cur_x += scale;
                PointF pr = new PointF(cx + cur_x, cy);
                PointF pl = new PointF(cx - cur_x, cy);
                gr.DrawString(counter.ToString(), font, pen_digits.Brush, pr);
                gr.DrawString("-" + counter.ToString(), font, pen_digits.Brush, pl);
            }

            Pen penR = new Pen(Color.Red, 2);
            gr.DrawLine(penR, cx, 0, cx, panel.Height);
            gr.DrawLine(penR, 0, cy, panel.Width, cy);
            PointF[] x_vec = new PointF[] { new PointF(cx, 0), new PointF(cx - 2, 5), new PointF(cx + 2, 5) };
            PointF[] y_vec = new PointF[] { new PointF(panel1.Width - 1, cy), new PointF(panel1.Width - 6, cy - 2), new PointF(panel1.Width - 6, cy + 2) };
            gr.DrawPolygon(penR, x_vec);
            gr.DrawPolygon(penR, y_vec);
            gr.DrawString("x", new Font("Arial", scale / 2), penR.Brush, new PointF(panel1.Width - 20, cy - 20));
            gr.DrawString("y", new Font("Arial", scale / 2), penR.Brush, new PointF(cx - 15, 0));
            
        }


        public void drawRasterization(Graphics gr, Panel panel, VScrollBar vsb, HScrollBar hsb, PointF begin, int radius)
        {
            watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var points = rasterizer.CircleBresenham(begin, radius);
            watch.Stop();
            label5.Text = watch.Elapsed.TotalMilliseconds.ToString();
            var cx = panel.Width / 2 + (hsb.Value - hsb.Maximum / 2) * shift;
            var cy = panel.Height / 2 - (vsb.Value - vsb.Maximum / 2) * shift;
            Pen pen = new Pen(Color.Black, 3);
            for (int i = 0; i < points.Length; i++)
            {
                Point p = new Point(points[i].X * scale + cx - shift, -points[i].Y * scale + cy - shift);
                var rect = new Rectangle(p, new Size(scale, scale));
                gr.DrawRectangle(pen, rect);
                gr.FillRectangle(bruh.Brush, rect);
            }
        }
        public void drawRasterization(Graphics gr, Panel panel, VScrollBar vsb, HScrollBar hsb, PointF begin, PointF end)
        {
            watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var points = rasterizer.Wu(begin, end);
            watch.Stop();
            label5.Text = watch.Elapsed.TotalMilliseconds.ToString();
            var cx = panel.Width / 2 + (hsb.Value - hsb.Maximum / 2) * shift;
            var cy = panel.Height / 2 - (vsb.Value - vsb.Maximum / 2) * shift;
            Pen pen;
            for (int i = 0; i < points.Count; i++)
            {
                pen = new Pen(Color.FromArgb((int)Math.Round(255 * points[i].Value), 0, 0, 0));
                int x, y;

                Point p = new Point((int)points[i].Key.X * scale + cx - shift, -(int)points[i].Key.Y * scale + cy - shift);
                var rect = new Rectangle(p, new Size(scale, scale));
                gr.DrawRectangle(pen, rect);
                gr.FillRectangle(pen.Brush, rect);
            }
        }
        public enum rasterization
        {
            Bresenham = 1,
            BresenhamCircle,
            DDA,
            Wu,
            Naive,
            CastlePitway,
        };
        public void drawRasterization(rasterization r, Graphics gr, Panel panel, VScrollBar vsb, HScrollBar hsb, PointF begin, PointF end)
        {
            Point[] points;
            switch (r)
            {
                case (rasterization.Bresenham):
                    {
                        watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        points = rasterizer.Bresenham(begin, end);
                        watch.Stop();
                        label5.Text = watch.Elapsed.TotalMilliseconds.ToString();
                        break;
                    }
                case (rasterization.DDA):
                    {
                        watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        points = rasterizer.DDA(begin, end);
                        watch.Stop();
                        label5.Text = watch.Elapsed.TotalMilliseconds.ToString();
                        break;
                    }
                case (rasterization.Naive):
                    {
                        watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        points = rasterizer.Naive(begin, end);
                        watch.Stop();
                        label5.Text = watch.Elapsed.TotalMilliseconds.ToString();
                        break;
                    }
                default:
                    {
                        watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        points = rasterizer.DDA(begin, end);
                        break;
                    }
            }
            var cx = panel.Width / 2 + (hsb.Value - hsb.Maximum / 2) * shift;
            var cy = panel.Height / 2 - (vsb.Value - vsb.Maximum / 2) * shift;
            Pen pen = new Pen(Color.Black, 3);
            for (int i = 0; i < points.Length; i++)
            {
                Point p = new Point(points[i].X * scale + cx - shift, -points[i].Y * scale + cy - shift);
                var rect = new Rectangle(p, new Size(scale, scale));
                gr.DrawRectangle(pen, rect);
                gr.FillRectangle(bruh.Brush, rect);
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            drawPlot(gr, panel1, vScrollBar1, hScrollBar1);

            if (circle)
            {
                drawRasterization(gr, panel1, vScrollBar1, hScrollBar1, center, radius);
                
            }
            else
            {
                if (radioButton1.Checked)
                    drawRasterization(rasterization.Naive, gr, panel1, vScrollBar1, hScrollBar1, start, end);
                if (radioButton2.Checked)
                    drawRasterization(rasterization.DDA, gr, panel1, vScrollBar1, hScrollBar1, start, end);
                if (radioButton3.Checked)
                    drawRasterization(rasterization.Bresenham, gr, panel1, vScrollBar1, hScrollBar1, start, end);
                drawLine(gr, panel1, vScrollBar1, hScrollBar1, start, end, Color.OrangeRed);
            }

            drawMarkup(gr, panel1, vScrollBar1, hScrollBar1);
           
        }
        public void drawPlot(Graphics gr, Panel panel, VScrollBar vsb, HScrollBar hsb)
        {
            Pen pen = new Pen(Color.Gray, 1);
            Pen pen_dash = new Pen(Color.LightGray, 1);
            pen_dash.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            gr.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            var cx = panel.Width / 2 + (hsb.Value - hsb.Maximum / 2) * shift;
            var cy = panel.Height / 2 - (vsb.Value - vsb.Maximum / 2) * shift;
            int cur_x = 0, cur_y = 0;
            if (cx <= panel.Width / 2)
            {
                while (cx + cur_x <= panel.Width)
                {
                    gr.DrawLine(pen, cx - cur_x, 0, cx - cur_x, panel.Height);
                    gr.DrawLine(pen, cx + cur_x, 0, cx + cur_x, panel.Height);
                    gr.DrawLine(pen_dash, cx - cur_x + shift, 0, cx - cur_x + shift, panel.Height);
                    gr.DrawLine(pen_dash, cx + cur_x - shift, 0, cx + cur_x - shift, panel.Height);
                    cur_x += scale;
                }
            }
            else
            {
                while (cx - cur_x >= 0)
                {
                    gr.DrawLine(pen, cx - cur_x, 0, cx - cur_x, panel.Height);
                    gr.DrawLine(pen, cx + cur_x, 0, cx + cur_x, panel.Height);
                    gr.DrawLine(pen_dash, cx - cur_x + shift, 0, cx - cur_x + shift, panel.Height);
                    gr.DrawLine(pen_dash, cx + cur_x - shift, 0, cx + cur_x - shift, panel.Height);
                    cur_x += scale;
                }
            }
            if (cy <= panel.Height / 2)
            {
                while (cy + cur_y <= panel.Height)
                {
                    gr.DrawLine(pen, 0, cy - cur_y, panel.Width, cy - cur_y);
                    gr.DrawLine(pen, 0, cy + cur_y, panel.Width, cy + cur_y);
                    gr.DrawLine(pen_dash, 0, cy - cur_y + shift, panel.Width, cy - cur_y + shift);
                    gr.DrawLine(pen_dash, 0, cy + cur_y - shift, panel.Width, cy + cur_y - shift);
                    cur_y += scale;

                }
            }
            else
            {
                while (cy - cur_y >= 0)
                {
                    gr.DrawLine(pen, 0, cy - cur_y, panel.Width, cy - cur_y);
                    gr.DrawLine(pen, 0, cy + cur_y, panel.Width, cy + cur_y);
                    gr.DrawLine(pen_dash, 0, cy - cur_y + shift, panel.Width, cy - cur_y + shift);
                    gr.DrawLine(pen_dash, 0, cy + cur_y - shift, panel.Width, cy + cur_y - shift);
                    cur_y += scale;

                }
            }
        }
        public void drawLine(Graphics gr, Panel panel, VScrollBar vsb, HScrollBar hsb, PointF start, PointF end, Color color)
        {
            var cx = panel.Width / 2 + (hsb.Value - hsb.Maximum / 2) * shift;
            var cy = panel.Height / 2 - (vsb.Value - vsb.Maximum / 2) * shift;
            PointF s = new PointF(start.X * scale + cx, -start.Y * scale + cy);
            PointF e = new PointF(end.X * scale + cx, -end.Y * scale + cy);
            Pen p = new Pen(color, (int)Math.Floor((decimal)shift / 2));
            gr.DrawLine(p, s, e);
        }
        public void drawCircle(Graphics gr, Panel panel, VScrollBar vsb, HScrollBar hsb, Point center, int radius, Color color)
        {
            var cx = panel.Width / 2 + (hsb.Value - hsb.Maximum / 2) * shift;
            var cy = panel.Height / 2 - (vsb.Value - vsb.Maximum / 2) * shift;
            gr.DrawEllipse(new Pen(color, (int)Math.Floor((decimal)shift / 2)), (center.X * scale) + cx - radius * scale, -center.Y * scale + cy - radius * scale, 2 * radius * scale, 2 * radius * scale);
        }
        private void clear(Panel panel)
        {
            panel.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start = new Point((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            end = new Point((int)numericUpDown3.Value, (int)numericUpDown4.Value);
            circle = false;
            clear(panel1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            center = new Point((int)numericUpDown5.Value, (int)numericUpDown6.Value);
            radius = (int)numericUpDown7.Value;
            circle = true;
            clear(panel1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            scale = (int)numericUpDown8.Value;
            shift = scale / 2;
            clear(panel1);
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            clear(panel1);
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            clear(panel1);
        }
    }
    public class Rasterizer
    {
        public Rasterizer()
        {

        }
        public void Swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }
        public void Swap(ref float x, ref float y)
        {
            float temp = x;
            x = y;
            y = temp;
        }
        public Point[] Bresenham(PointF begin, PointF end)
        {
            var x0 = (int)begin.X;
            var y0 = (int)begin.Y;
            var x1 = (int)end.X;
            var y1 = (int)end.Y;

            List<Point> list = new List<Point>();
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); // Проверяем рост отрезка по оси икс и по оси игрек
                                                               // Отражаем линию по диагонали, если угол наклона слишком большой
            if (steep)
            {
                Swap(ref x0, ref y0); // Перетасовка координат вынесена в отдельную функцию для красоты
                Swap(ref x1, ref y1);
            }
            // Если линия растёт не слева направо, то меняем начало и конец отрезка местами
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            int dx = x1 - x0;
            int dy = y1 - y0;
            int error = dx / 2; // Здесь используется оптимизация с умножением на dx, чтобы избавиться от лишних дробей
            int ystep = (y0 < y1) ? 1 : -1; // Выбираем направление роста координаты y
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                list.Add(new Point(steep ? y : x, steep ? x : y)); // Не забываем вернуть координаты на место
                error -= Math.Abs(dy);
                if (error < 0)
                {
                    y += ystep;
                    error += Math.Abs(dx);
                }
            }
            return list.ToArray();
        }

        public Point[] Naive(PointF begin, PointF end)
        {
            var x0 = (int)begin.X;
            var y0 = (int)begin.Y;
            var x1 = (int)end.X;
            var y1 = (int)end.Y;

            List<Point> list = new List<Point>();
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            // Если линия растёт не слева направо, то меняем начало и конец отрезка местами
            float dx = x1 - x0;
            if (dx == 0)
            {
                if (y1 < y0)
                {
                    Swap(ref y1, ref y0);
                }
                for (int y = y0; y <= y1; y++)
                {
                    list.Add(new Point(x1, y));
                }
                return list.ToArray();
            }
            float dy = y1 - y0;
            var b = (float)y0 - (float)x0 * dy / dx;
            bool steep = false;
            if (Math.Abs(dy) > Math.Abs(dx))
            {
                steep = true;
            }

            list.Add(new Point(x0, y0));
            list.Add(new Point(x1, y1));
            float temp;
            if (!steep)
            {
                if (x1 < x0)
                {
                    Swap(ref x1, ref x0);
                }
                for (float x = x0 + 1; x < x1; x++)
                {
                    list.Add(new Point((int)x, (int)Math.Round(dy / dx * x + b))); // Не забываем вернуть координаты на место
                }
            }
            else
            {
                if (y1 < y0)
                {
                    Swap(ref y1, ref y0);
                }
                for (float y = y0 + 1; y < y1; y++)
                {
                    list.Add(new Point((int)Math.Round((y - b) * dx / dy), (int)y)); // Не забываем вернуть координаты на место
                }
            }
            return list.ToArray();
        }
        public Point[] CircleBresenham(PointF center, int radius)
        {
            var x0 = (int)center.X;
            var y0 = (int)center.Y;
            List<Point> list = new List<Point>();
            int x = radius;
            int y = 0;
            int radiusError = 1 - x;
            while (x >= y)
            {
                list.Add(new Point(x + x0, y + y0));
                list.Add(new Point(y + x0, x + y0));
                list.Add(new Point(-x + x0, y + y0));
                list.Add(new Point(-y + x0, x + y0));
                list.Add(new Point(-x + x0, -y + y0));
                list.Add(new Point(-y + x0, -x + y0));
                list.Add(new Point(x + x0, -y + y0));
                list.Add(new Point(y + x0, -x + y0));
                y++;
                if (radiusError < 0)
                {
                    radiusError += 2 * y + 1;
                }
                else
                {
                    x--;
                    radiusError += 2 * (y - x + 1);
                }
            }
            return list.ToArray();
        }

        public List<KeyValuePair<PointF, float>> Wu(PointF begin, PointF end)
        {
            float ipart(float x)
            {
                return (int)Math.Floor(x);
            };

            float round(float x)
            {
                return ipart(x + 0.5f);
            };

            float fpart(float x)
            {
                return x - (float)Math.Floor(x);
            };

            float rfpart(float x)
            {
                return 1 - fpart(x);
            };
            bool reflOx = false;
            bool reflOy = false;
            List<KeyValuePair<PointF, float>> list = new List<KeyValuePair<PointF, float>>();
            var x0s = (int)begin.X;
            var y0s = (int)begin.Y;
            var x1s = (int)end.X;
            var y1s = (int)end.Y;
            var x0 = 0;
            var y0 = 0;
            var x1 = x1s - x0s;
            var y1 = y1s - y0s;

            if (x1 < 0)
            {
                reflOy = true;
                x1 *= -1;
            }
            if (y1 < 0)
            {
                reflOx = true;
                y1 *= -1;
            }
            var steep = Math.Abs(y1) > Math.Abs(x1);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            void add(float x, float yy, float intensity)
            {
                if (intensity < 0 || intensity > 1)
                {
                    return;
                }
                if (reflOy)
                {
                    if (!steep)
                    {
                        x *= -1;
                    }
                    else
                    {
                        yy *= -1;
                    }
                }
                if (reflOx)
                {
                    if (!steep)
                    {
                        yy *= -1;
                    }
                    else
                    {
                        x *= -1;
                    }

                }
                if (!steep)
                {
                    x += x0s;
                    yy += y0s;
                }
                else
                {
                    yy += x0s;
                    x += y0s;
                }
                if (steep)
                {
                    Swap(ref x, ref yy);
                }
                if (end.X > begin.X)
                {
                    x = (int)Math.Floor(x);
                }
                else
                {
                    x = (int)Math.Ceiling(x);
                }
                if (end.Y > begin.Y)
                {
                    yy = (int)Math.Floor(yy);
                }
                else
                {
                    yy = (int)Math.Ceiling(yy);
                }
                list.Add(new KeyValuePair<PointF, float>(new PointF(x, yy), intensity));
            }

            float dx = x1 - x0;
            float dy = y1 - y0;
            add(x0, y0, 1);
            add(x1, y1, 1);

            float gradient = dy / dx;
            float y = y0 + gradient;
            for (var x = x0 + 1; x < x1; x++)
            {
                add(x, y, rfpart(y));
                add(x, y + 1, fpart(y));
                y += gradient;
            }

            return list;

        }

        public Point[] CastlePitway(PointF begin, PointF end)
        {
            List<Point> list = new List<Point>();
            string reverse(string s)
            {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
            // Координаты концов отрезка - (0,0) и (a,b)
            int x, y;
            y = (int)end.Y;
            x = (int)end.X - (int)end.Y;
            string m1 = "s";
            string m2 = "d";
            while (x != y)
            {
                if (x > y)
                {
                    x -= y;
                    m2 = m1 + reverse(m2);
                }
                else
                {
                    y -= x;
                    m1 = m2 + reverse(m1);
                }

            }
            var m = m2 + reverse(m1);
            int cur_X = (int)begin.X;
            int cur_y = (int)begin.Y;
            list.Add(new Point(cur_X, cur_y));
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i] == 's')
                {
                    cur_X++;
                    list.Add(new Point(cur_X, cur_y));
                }
                else
                {
                    cur_X++;
                    cur_y++;
                    list.Add(new Point(cur_X, cur_y));
                }
            }
            list.Add(new Point((int)end.X, (int)end.Y));
            return list.ToArray();
        }

        public Point[] DDA(PointF begin, PointF end)
        {
            var x1 = (int)begin.X;
            var y1 = (int)begin.Y;
            var x2 = (int)end.X;
            var y2 = (int)end.Y;
            int deltaX = (int)Math.Abs(x2 - x1);
            int deltaY = (int)Math.Abs(y2 - y1);

            int signX = (x1 < x2) ? 1 : -1;
            int signY = y1 < y2 ? 1 : -1;
            int error = deltaX - deltaY;
            List<Point> list = new List<Point>();
            list.Add(new Point(x2, y2));
            while (x1 != x2 || y1 != y2)
            {
                list.Add(new Point(x1, y1));
                int error2 = error * 2;
                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }
            return list.ToArray();
        }
    }
}
