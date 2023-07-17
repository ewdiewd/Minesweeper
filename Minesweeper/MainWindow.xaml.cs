using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   static string mode="Hard";
        Field fields = new Field(mode);
        List<Field> FIELDS=new List<Field>();
        List<int> LockList = new List<int>();
        List<Cell>CellList =new List<Cell>();
        int open;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            FIELDS.Add(fields);
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < FIELDS[0].ModeGrid; i++)
            {
                var temp = new Button(); var cell = new Cell(FIELDS[0].Fl[i]);
                temp.MouseRightButtonDown += Temp_MouseRightButtonDown;
                temp.Click += Temp_Click;
                temp.MouseDoubleClick += Temp_MouseDoubleClick;
                temp.Content = "";// + FIELDS[0].Fl[i];
                temp.Width = 32; temp.Height = 32;
                temp.Tag = i;
                fed.Children.Add(temp);

                temp.FontWeight = FontWeights.Bold;
                temp.SetBinding(Button.ContentProperty,new Binding("Content") { Source = cell });
                temp.SetBinding(Button.ForegroundProperty,new Binding("NumberColor") { Source = cell });
                CellList.Add(cell);
                switch (mode)
                {
                    case "Easy":Mine.Text = "10"; open =71; break;
                    case "Normal": Mine.Text = "40"; open=216; break;
                    case "Hard": Mine.Text = "99";open=381 ; break;
                    default:
                        break;
                }
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            int a; 
                a = Convert.ToInt32(Time.Text);
                Time.Text = (a + 1).ToString();

        }
        private void Temp_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)//双击
        {
            var temp = (Button)sender; var content = FIELDS[0].Fl[(int)temp.Tag];
            var All=ints((int)temp.Tag, FIELDS[0].f);
            foreach (var item in LockList)//除去所有插旗的
            {
                All.RemoveAll(x => x == item);
            }
            if (All.Count + content==8 || All.Count + content == 5 || All.Count+content==3 ||All.Count + content == 4)//数字与其周围非雷的之和
            {
                foreach (var item in All)
                {
                    Temp_Click(fed.Children[item] as Button, null);
                    if (FIELDS[0].state == false) break;
                }
            }
        }


        private void Temp_Click(object sender, RoutedEventArgs e)
        {
            var temp = (Button)sender; var FielNumber = FIELDS[0].Fl[(int)temp.Tag];
            var cell = CellList[(int)temp.Tag];
            if (!FIELDS[0].state)//计时器
            {
                FIELDS[0].state = true;
                timer.Start();
            }
            OpenTheZero((int)temp.Tag);
            if (cell.isMine)
            {
                timer.Stop();
                if (false)
                {
                    New_game();
                }
                else
                {
                    foreach (var item in CellList)
                    {
                        if (item.isMine)
                        {
                            item.Content = "💣";
                        }
                    }
                    MessageBox.Show("Boom!");
                    FIELDS[0].state = false;
                    newGameButton.Visibility = Visibility.Visible;
                    fed.IsEnabled = false;
                    FIELDS[0].state = false;

                }
            }
            if (CellList.Count(c => c.Ct == CellType.Open) == open)
            {
                timer.Stop();
                foreach (var item in CellList)
                {
                    if (item.Ct == CellType.Close)
                    {
                        item.Content = "❀";
                    }
                    else if (item.Ct == CellType.Guess)
                    {
                        item.Content = "👍";
                    }
                }
                MessageBox.Show($"恭喜！您的成绩是{Time.Text}秒！");
                FIELDS[0].state = false;
                newGameButton.Visibility = Visibility.Visible;
                fed.IsEnabled = false;
                FIELDS[0].state = false;
            }
        }
        private void Temp_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var temp = (Button)sender;var cell = CellList[(int)temp.Tag];
            if (temp.Content == "!")
            {
                temp.Content = "?";
                cell.Ct = CellType.Guess;
                temp.Click -= Temp_Click;//猜测
                temp.MouseDoubleClick -= Temp_MouseDoubleClick;
                Mine.Text = (Convert.ToInt16(Mine.Text) + 1).ToString();
            }
            else if (temp.Content == "?")
            {
                temp.Content = "";
                cell.Ct = CellType.Close;
                temp.Click += Temp_Click;//恢复左键点击
                temp.SetBinding(Button.ContentProperty, new Binding("Content") { Source = cell });
                LockList.Remove((int)temp.Tag);
                temp.MouseDoubleClick += Temp_MouseDoubleClick;
            }
            else
            {
                temp.Content = "!";
                cell.Ct = CellType.Flag;
                LockList.Add((int)temp.Tag);
                temp.Click -= Temp_Click;//插旗
                temp.MouseDoubleClick -= Temp_MouseDoubleClick;
                Mine.Text=(Convert.ToInt16(Mine.Text)-1).ToString();
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            New_game();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            New_game();
            ((Button)sender).Visibility = Visibility.Hidden;
        }

        private void Mode_Click(object sender, RoutedEventArgs e)
        {
            bool Change =false;
            if (sender is MenuItem )
            {
                var item = new MenuItem();
                item = (MenuItem)sender;
                mode = item.Header.ToString();
                Change = (mode != FIELDS[0].difficulty);
            }

            if (Change)//如果没有更换难度，格子不需要变动,也不需要更新
            {
                FIELDS[0].width = XY(mode, out int Height);
                FIELDS[0].height = Height;
                this.mainForm.SetBinding(FrameworkElement.WidthProperty, new Binding("width") { Source = FIELDS[0] });
                this.mainForm.SetBinding(FrameworkElement.HeightProperty, new Binding("height") { Source = FIELDS[0] });
                New_game();
                this.modeBlock.SetBinding(TextBlock.TextProperty, new Binding("difficulty") { Source = FIELDS[0] });
            }


        }

        void OpenTheZero(int xy)
        {
            var cell = CellList[xy]; var temp = (Button)fed.Children[xy];
            if (cell.Ct == CellType.Flag || cell.isMine || cell.Ct == CellType.Open || cell.Ct == CellType.Guess)
            {
                return;
            }
            cell.Ct = CellType.Open;
            temp.Background = null;
            temp.Click -= Temp_Click;
            temp.MouseRightButtonDown -= Temp_MouseRightButtonDown;
            if (cell.tempContent == "0")
            {
                var All = ints(xy, FIELDS[0].f);
                foreach (var item in All)
                {
                    OpenTheZero(item);
                }
            }
            else
            { return; }

        }
        public void New_game()
        {
            fed.IsEnabled = true;
            Field fields = new Field(mode);
            FIELDS.Clear();
            FIELDS.Add(fields);
            fed.Children.Clear();
            LockList.Clear();
            CellList.Clear();
            Time.Text = "0";
            timer.Stop();
            Window_Loaded(fed, null);
        }
        public int XY(string mode,out int Height) 
        {
            int Width = 960; Height=512;
            switch (mode)
            {
                case "Easy":
                    Width = 325; Height = 367;
                    break;
                case "Normal":
                    Width = 548; Height = 590;
                    break;
                case "Hard":
                    Width = 995; Height = 590;
                    break;
                default:
                    break;
            }
            return Width;
        }
        public void Find0(int xy )
        {
            var All = ints(xy, FIELDS[0].f);//int Allcount = All.Count;
            All.Remove(xy);
            foreach (var item in All)
            {
                var temp = (fed.Children[item] as Button);
                if (FIELDS[0].Fl[item]==0)
                {
                    temp.Content = "";
                }
                else
                {
                    temp.Content = FIELDS[0].Fl[item];
                }
                if (temp.Background != null)
                {
                    open -= 1;
                }
                temp.Background = null;
                temp.Click -= Temp_Click;
                temp.MouseRightButtonDown -= Temp_MouseRightButtonDown;
            }
        }
        public List<int> ints(int Input, int[,] Tempfirld)
        {
            List<int> Output = new List<int>();
            int Row = Tempfirld.GetLength(0); int Col = Tempfirld.GetLength(1);
            int r = Input / Col;//row
            int c = Input % Col;//col
            //MessageBox.Show(r.ToString());
            //MessageBox.Show(c.ToString());
            if (Input % Col != 0 && (Input + 1) % Col != 0 && Input > Col && Input < Col * (Row - 1))//除去四周
            {
                for (int i = r - 1; i < r - 1 + 3; i++)
                {
                    for (int j = c - 1; j < c - 1 + 3; j++)
                    {
                        int temp = i * Tempfirld.GetLength(1) + j;
                        if (temp != Input)
                        {
                            Output.Add(temp);
                        }
                    }
                }

            }
            else if (Input % Col == 0)//左列
            {
                for (int i = r - 1; i < r - 1 + 3; i++)
                {
                    for (int j = c; j < c + 2; j++)
                    {
                        int temp = i * Tempfirld.GetLength(1) + j;
                        if (temp != Input)
                        {
                            Output.Add(temp);
                        }
                    }
                }
            }
            else if ((Input + 1) % Col == 0)//右列
            {
                for (int i = r - 1; i < r - 1 + 3; i++)
                {
                    for (int j = c - 1; j < c + 1; j++)
                    {
                        int temp = i * Tempfirld.GetLength(1) + j;
                        if (temp != Input)
                        {
                            Output.Add(temp);
                        }
                    }
                }
            }
            else if (Input < Col) //上列
            {
                for (int i = r; i < r + 2; i++)
                {
                    for (int j = c - 1; j < c - 1 + 3; j++)
                    {
                        int temp = i * Tempfirld.GetLength(1) + j;
                        if (temp != Input)
                        {
                            Output.Add(temp);
                        }
                    }
                }
            }
            else if (Input > Col * (Row - 1))//下列
            {
                for (int i = r - 1; i < r + 1; i++)
                {
                    for (int j = c - 1; j < c - 1 + 3; j++)
                    {
                        int temp = i * Tempfirld.GetLength(1) + j;
                        if (temp != Input)
                        {
                            Output.Add(temp);
                        }
                    }
                }
            }
            Output.RemoveAll(x => x < 0 || x > (Col * Row)-1);


            return Output;
        }

    }
}
