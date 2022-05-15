using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Reaction
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TimeSpan> ЛистИнтервалов = new List<TimeSpan>();
        // List<double> ЛистДаблов = new List<double>();
        double растояниеДокнопки;

        DateTime текущееВРемя = DateTime.Now;
        TimeSpan интервал;
        вДанные данные = new вДанные();
        Timer таймерПлохойРеакции = new Timer();
        Timer таймерВызова = new Timer();
        Random рандом = new Random();
        int[] числа = new int[4];
        Brush стандартныйЦвет;


        public MainWindow()
        {
            InitializeComponent();
            стандартныйЦвет = кнопка_База.Background;
            таймерВызова.Interval = 100;
            таймерВызова.Elapsed += ТаймерВызова_Elapsed;
            таймерВызова.AutoReset = false;
            таймерПлохойРеакции.Interval = 2000;
            таймерПлохойРеакции.Elapsed += ТаймерПлохойРеакции_Elapsed;
            таймерПлохойРеакции.AutoReset = false;
        }

        #region ____________________________________СОБЫТИЯ

        private void ТаймерВызова_Elapsed(object sender, ElapsedEventArgs e)
        {
            текущееВРемя = DateTime.Now;
            int случайное = рандом.Next(1, 5);
            switch (случайное)
            {
                case 1: ИзменитьКнопку(кнопка_Верх); break;
                case 2: ИзменитьКнопку(кнопка_Низ); break;
                case 3: ИзменитьКнопку(кнопка_Лево); break;
                case 4: ИзменитьКнопку(кнопка_Право); break;
            }
            История($"{DateTime.Now:T} ТаймерВызова  - функция в разработке");
            таймерВызова.Stop();
            таймерПлохойРеакции.Start();
            ЗаполнитьЛейбл();
        }

        private void ТаймерПлохойРеакции_Elapsed(object sender, ElapsedEventArgs e)
        {
            История($" НЕ УСПЕЛ, прошло {таймерПлохойРеакции.Interval}  - функция в разработке"); таймерПлохойРеакции.Stop(); ЗаполнитьЛейбл();
        }

        #endregion события

        #region  ____________________________________МЕТОДЫ

        void НажатиеНаКнопку(Object кнопка)
        {
            Button даКнопка = кнопка as Button;

            if (даКнопка.Background == Brushes.Red)
            {
                даКнопка.Background = стандартныйЦвет; таймерПлохойРеакции.Stop(); данные.новыйЦикл = true;
                интервал = DateTime.Now.Subtract(текущееВРемя);
                double дабл = интервал.TotalMilliseconds;
                ЛистИнтервалов.Append(интервал);
                данные.ЛистДаблов.Add(дабл);
                if (данные.ЛистДаблов.Count > 0)
                {
                    данные.среднее = данные.ЛистДаблов.Average();
                    данные.максимальное = данные.ЛистДаблов.Max();
                    данные.минимальное = данные.ЛистДаблов.Min();
                    данные.попаданий = данные.ЛистДаблов.Count();
                }

                История($"попал {интервал}   реакция={дабл}   среднее={данные.среднее}   миниум={данные.минимальное}   максимум={данные.максимальное}");
            }
            else { История($"мимо - функция в разработке"); }
            ЗаполнитьЛейбл();

        }

        void ЗаполнитьЛейбл()
        {
            Dispatcher.Invoke(() =>
            {
                label_инфо.Content = $"наБазе={данные.наБазе}   новыйЦикл={данные.новыйЦикл}\n" +
                        $"max={данные.максимальное}\n" +
                        $"среднее={данные.среднее}\n" +
                        $"min={данные.минимальное}\n" +
                        $"попаданий={данные.попаданий}\n" +
                        $"среднееРастояниеДоКнопки={растояниеДокнопки}";
            });
        }

        void ИзменитьКнопку(Button кнопка)
        {
            Dispatcher.Invoke(() => { кнопка.Background = Brushes.Red; });
        }

        void История(string текст)
        {
            Dispatcher.Invoke(() =>
            {
                textbox_история.AppendText(текст + Environment.NewLine);
                textbox_история.ScrollToEnd();
            });
        }


        #endregion методы


        #region  ____________________________________КНОПКИ
        private void кнопка_Лево_Click(object sender, RoutedEventArgs e)
        {
            НажатиеНаКнопку(sender);
        }
        private void кнопка_Верх_Click(object sender, RoutedEventArgs e)
        {
            НажатиеНаКнопку(sender);
        }
        private void кнопка_Право_Click(object sender, RoutedEventArgs e)
        {
            НажатиеНаКнопку(sender);
        }
        private void кнопка_Низ_Click(object sender, RoutedEventArgs e)
        {
            НажатиеНаКнопку(sender);
        }
        private void кнопка_Старт_Click(object sender, RoutedEventArgs e) { таймерВызова.Start(); текущееВРемя = DateTime.Now; }
        private void кнопка_Стоп_Click(object sender, RoutedEventArgs e) { таймерВызова.Stop(); История($" размер {grid_Направления2.ActualWidth} {grid_Направления2.ActualHeight}"); }
        private void кнопка_Чистить_Click(object sender, RoutedEventArgs e) { textbox_история.Clear(); данные = new вДанные(); ЗаполнитьЛейбл(); }
        private void кнопка_База_MouseLeave(object sender, MouseEventArgs e)
        {
            кнопка_База.Background = Brushes.Yellow;
            данные.наБазе = false;
            ЗаполнитьЛейбл();
        }
        private void кнопка_База_MouseEnter(object sender, MouseEventArgs e)
        {
            кнопка_База.Background = стандартныйЦвет;
            данные.наБазе = true;
            if (данные.новыйЦикл)
            {
                таймерВызова.Start(); данные.новыйЦикл = false;
            }
            ЗаполнитьЛейбл();
        }
        private void ГлавноеОкно_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            растояниеДокнопки = (grid_Направления2.ActualWidth + grid_Направления2.ActualHeight) / 4 - 20; История($" размер {grid_Направления2.ActualWidth} {grid_Направления2.ActualHeight}");
        }
        #endregion кнопки 
    }
}
