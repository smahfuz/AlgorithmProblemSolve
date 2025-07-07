using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic; // InputBox এর জন্য

namespace DijkstraDemo
{
    public partial class Form1 : Form
    {
        const int rows = 10, cols = 10;
        const int cellSize = 40;
        Point start = new Point(0, 0);
        Point end = new Point(9, 9);

        bool[,] obstacles = new bool[rows, cols];
        int[,] distances = new int[rows, cols];
        int[,] cellCost = new int[rows, cols];
        Point[,] previous = new Point[rows, cols];
        bool pathFound = false;

        // UI Controls
        RadioButton rbDefault, rbCustom;
        Button btnRandom, btnRefresh;

        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = cols * cellSize + 200;  // Extra space for controls
            this.Height = rows * cellSize + 60;

            InitControls();

            SetDefaultCosts();
            RunDijkstra();
        }

        private void InitControls()
        {
            // Radio Button Default Cost = 1
            rbDefault = new RadioButton()
            {
                Text = "Default Cost = 1",
                Location = new Point(cols * cellSize + 20, 20),
                AutoSize = true,
                Checked = true
            };
            rbDefault.CheckedChanged += (s, e) =>
            {
                if (rbDefault.Checked)
                {
                    SetDefaultCosts();
                    RunDijkstra();
                }
            };
            this.Controls.Add(rbDefault);

            // Radio Button Customizable Cost
            rbCustom = new RadioButton()
            {
                Text = "Customizable Cost",
                Location = new Point(cols * cellSize + 20, 50),
                AutoSize = true
            };
            this.Controls.Add(rbCustom);

            // Random Button
            btnRandom = new Button()
            {
                Text = "Random Costs",
                Location = new Point(cols * cellSize + 20, 90),
                Width = 120
            };
            btnRandom.Click += (s, e) =>
            {
                SetRandomCosts();
                RunDijkstra();
            };
            this.Controls.Add(btnRandom);

            // Refresh Button
            btnRefresh = new Button()
            {
                Text = "Refresh (Reset)",
                Location = new Point(cols * cellSize + 20, 130),
                Width = 120
            };
            btnRefresh.Click += (s, e) =>
            {
                ClearObstacles();
                SetDefaultCosts();
                RunDijkstra();
            };
            this.Controls.Add(btnRefresh);
        }

        private void SetDefaultCosts()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    cellCost[r, c] = 1;
        }

        private void SetRandomCosts()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    cellCost[r, c] = rnd.Next(1, 10); // Random cost between 1 and 9
        }

        private void ClearObstacles()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    obstacles[r, c] = false;
        }

        private void RunDijkstra()
        {
            var queue = new PriorityQueue<Point>();
            pathFound = false;

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    distances[r, c] = int.MaxValue;
                    previous[r, c] = Point.Empty;
                }

            distances[start.Y, start.X] = 0;
            queue.Enqueue(start, 0);

            int[][] dirs = { new[] { 0, 1 }, new[] { 1, 0 }, new[] { -1, 0 }, new[] { 0, -1 } };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == end)
                {
                    pathFound = true;
                    break;
                }

                foreach (var dir in dirs)
                {
                    int nx = current.X + dir[0];
                    int ny = current.Y + dir[1];

                    if (nx >= 0 && ny >= 0 && nx < cols && ny < rows && !obstacles[ny, nx])
                    {
                        int costToAdd = cellCost[ny, nx];
                        // If Default Cost is checked, force cost=1 ignoring current cellCost
                        if (rbDefault.Checked) costToAdd = 1;

                        int newDist = distances[current.Y, current.X] + costToAdd;
                        if (newDist < distances[ny, nx])
                        {
                            distances[ny, nx] = newDist;
                            previous[ny, nx] = current;
                            queue.Enqueue(new Point(nx, ny), newDist);
                        }
                    }
                }
            }

            this.Invalidate(); // redraw
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Rectangle rect = new Rectangle(c * cellSize, r * cellSize, cellSize, cellSize);
                    Brush brush = Brushes.White;

                    if (obstacles[r, c]) brush = Brushes.Black;
                    else if (new Point(c, r) == start) brush = Brushes.Green;
                    else if (new Point(c, r) == end) brush = Brushes.Red;
                    else if (pathFound && IsPathCell(c, r)) brush = Brushes.Yellow;

                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(Pens.Gray, rect);

                    // draw cost
                    if (!obstacles[r, c])
                    {
                        string text = cellCost[r, c].ToString();
                        var font = new Font("Arial", 10);
                        var size = g.MeasureString(text, font);
                        var textPos = new PointF(
                            c * cellSize + (cellSize - size.Width) / 2,
                            r * cellSize + (cellSize - size.Height) / 2
                        );
                        g.DrawString(text, font, Brushes.Black, textPos);
                    }
                }
            }
        }

        private bool IsPathCell(int x, int y)
        {
            Point p = end;
            while (previous[p.Y, p.X] != Point.Empty)
            {
                if (previous[p.Y, p.X] == new Point(x, y))
                    return true;
                p = previous[p.Y, p.X];
            }
            return false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
      
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            int x = e.X / cellSize;
            int y = e.Y / cellSize;

            if (x >= 0 && y >= 0 && x < cols && y < rows)
            {
                if (new Point(x, y) != start && new Point(x, y) != end)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        obstacles[y, x] = !obstacles[y, x];
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        if (rbCustom.Checked && !obstacles[y, x])
                        {
                            string input = Interaction.InputBox(
                                $"Set cost for cell ({x}, {y})",
                                "Enter Cell Cost",
                                cellCost[y, x].ToString());

                            if (int.TryParse(input, out int newCost) && newCost > 0)
                            {
                                cellCost[y, x] = newCost;
                            }
                        }
                    }

                    RunDijkstra();
                }
            }
        }
    }

    public class PriorityQueue<T>
    {
        private List<(T item, int priority)> data = new();

        public void Enqueue(T item, int priority)
        {
            data.Add((item, priority));
            data.Sort((a, b) => a.priority.CompareTo(b.priority));
        }

        public T Dequeue()
        {
            var item = data[0].item;
            data.RemoveAt(0);
            return item;
        }

        public int Count => data.Count;
    }
}
