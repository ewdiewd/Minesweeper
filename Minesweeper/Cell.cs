using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Minesweeper
{
    public class Cell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private CellType ct;
        public bool isMine=false;
        private string content;
        public string tempContent;//储存数字
        private Brush numberColor=new SolidColorBrush(Colors.Black);
        public Brush NumberColor { get { return numberColor; } set { numberColor = value; OnPropertyChanged("numberColor");} }
        public string Content{ get { return content; } set { content = value; OnPropertyChanged("content"); } }
        public CellType Ct { get { return ct; } set { ct = value; OnPropertyChanged("ct"); upcontent(); } }

        public Cell(int Number)
        {
            if (Number == 9) isMine = true;
            ct = CellType.Close;
            tempContent = Number.ToString();
        }

        protected void upcontent()//更新属性
        {
            if (ct == CellType.Open)
            {
                    switch (tempContent)
                    {
                        case "9": Content = "💣"; NumberColor= new SolidColorBrush(Colors.Red); break;
                        case "0": Content = string.Empty; break;
                        default: Content = tempContent;
                        upcolor();
                         break;

                    }
            }

        }
        protected void upcolor() //更新颜色
        {
                SolidColorBrush NColor=new SolidColorBrush(Color.FromRgb(243, 255, 0));
                switch (tempContent)
            {
                case "1": NColor.Color = Color.FromRgb(0, 80, 255);break;
                case "2": NColor.Color = Color.FromRgb(14, 194, 70); break;
                case "3": NColor.Color = Color.FromRgb(212, 68, 68); break;
                case "4": NColor.Color = Color.FromRgb(61, 0, 181); break;
                case "5": NColor.Color = Color.FromRgb(187, 17, 10); break;
                case "6": NColor.Color = Color.FromRgb(101, 109, 206); break;
                case "7": NColor.Color = Color.FromRgb(183, 13, 198); break;
                case "8": NColor.Color = Color.FromRgb(243, 255, 0); break;
                default:
                    break;
            }
                NumberColor = NColor;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) //通知方法
        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        }
    }
    public enum CellType {Close, Open, Flag,Guess }
}