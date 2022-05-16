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
        double растояниеДокнопки, лимитВремени = 2000, задержкаПодсветки = 1000;

        bool таймерНаСтарт = false, кнопкаПодсвечена = false, ограничениеВремени = false, ускорениеПодсветки = false;


        DateTime текущееВРемя;
        TimeSpan интервал;
        вДанные данные = new вДанные();
        Timer таймерПлохойРеакции = new Timer();
        Timer таймерПодсветки = new Timer();
        Random рандом = new Random();
        int[] числа = new int[4];
        Brush стандартныйЦвет;


        public MainWindow()
        {
            InitializeComponent();
            стандартныйЦвет = кнопка_База.Background;
            таймерПодсветки.Interval = задержкаПодсветки;
            таймерПодсветки.Elapsed += ТаймерВызова_Elapsed;
            таймерПодсветки.AutoReset = false;
            таймерПлохойРеакции.Interval = лимитВремени;
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
            кнопкаПодсвечена = true;
            таймерПодсветки.Stop();

            if (ограничениеВремени) // если включен режим ограничения времени
            {
                double урезатьНа = (лимитВремени - данные.среднее) * 0.2; // урезать лимит на 20% от интервала между лимитом и средней задержкой
                лимитВремени -= урезатьНа;
                таймерПлохойРеакции.Interval = лимитВремени; //глюк похоже, иногда при изменении интервала таймер сам запускается что ли
                таймерПлохойРеакции.Start();
            }
            ЗаполнитьЛейбл();
        }

        private void ТаймерПлохойРеакции_Elapsed(object sender, ElapsedEventArgs e)
        {
            данные.опаздание++;
            История($" НЕ УСПЕЛ, прошло {таймерПлохойРеакции.Interval}  - функция в разработке"); таймерПлохойРеакции.Stop(); ЗаполнитьЛейбл();
        }

        #endregion события

        #region  ____________________________________МЕТОДЫ

        void НажатиеНаКнопку(Object кнопка)
        {
            Button даКнопка = кнопка as Button;

            if (кнопкаПодсвечена && даКнопка.Background == Brushes.Red)
            {
                if (таймерПлохойРеакции.Enabled) // если таймер плохой реакции ещё идет, значит нажато вовремя
                {
                    таймерПлохойРеакции.Stop(); данные.вовремя++;

                }
                кнопкаПодсвечена = false;
                даКнопка.Background = стандартныйЦвет;
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
                    Dispatcher.Invoke(() => { slider_реакция.Value = данные.ЛистДаблов.Average(); slider_реакция.ToolTip = $"среднее время реакции {данные.ЛистДаблов.Average()} мс"; });
                }

                История($"{DateTime.Now:HH:mm:ss_fff} реакция последняя={дабл}   средняя={данные.среднее}   миниум={данные.минимальное}   максимум={данные.максимальное}");


            }
            else if (кнопкаПодсвечена && даКнопка.Background != Brushes.Red) { История($"мимо - функция в разработке"); данные.ошибок++; }            
            ЗаполнитьЛейбл();

        }

        void ЗаполнитьЛейбл()
        {
            Dispatcher.Invoke(() =>
            {
                label_инфо.Content = $"max={данные.максимальное} мс\n" +
                        $"среднее={данные.среднее} мс\n" +
                        $"min={данные.минимальное} мс\n" +
                        $"попаданий={данные.попаданий}   вовремя={данные.вовремя}   опазданий={данные.опаздание}\n" +
                        $"ошибок={данные.ошибок}   промахов={данные.промах}\n" +
                        $"среднееРастояниеДоКнопки={растояниеДокнопки}\n" +
                        $"таймерВызова.Enabled={таймерПодсветки.Enabled}   наБазе={данные.наБазе}\n" +
                        $"таймерПлохойРеакции.Enabled={таймерПлохойРеакции.Enabled}\n" +
                        $"лимитВремени={лимитВремени}   задержкаПодсветки={задержкаПодсветки}";
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

        private void кнопка_Старт_Click(object sender, RoutedEventArgs e) { таймерНаСтарт = true; кнопка_База.Background = Brushes.Green; textbox_история.Clear(); }
        private void кнопка_Стоп_Click(object sender, RoutedEventArgs e) { таймерНаСтарт = false; таймерПодсветки.Stop(); }
        private void кнопка_Лево_Click(object sender, RoutedEventArgs e) { НажатиеНаКнопку(sender); }
        private void кнопка_Верх_Click(object sender, RoutedEventArgs e) { НажатиеНаКнопку(sender); }
        private void кнопка_Право_Click(object sender, RoutedEventArgs e) { НажатиеНаКнопку(sender); }
        private void кнопка_Низ_Click(object sender, RoutedEventArgs e) { НажатиеНаКнопку(sender); }
        private void кнопка_Чистить_Click(object sender, RoutedEventArgs e) { textbox_история.Clear(); slider_реакция.Value = 0; данные = new вДанные(); ЗаполнитьЛейбл(); таймерПлохойРеакции.Interval = лимитВремени; }
        private void кнопка_База_MouseLeave(object sender, MouseEventArgs e)
        {
            кнопка_База.Background = Brushes.Yellow;
            данные.наБазе = false;
            if (!кнопкаПодсвечена)//если курсор убирается до того как кнопк подсветится, то останавливать таймер
            {
                таймерПодсветки.Stop();
            }
            ЗаполнитьЛейбл();
        }
        private void кнопка_База_MouseEnter(object sender, MouseEventArgs e)
        {
            кнопка_База.Background = стандартныйЦвет;
            данные.наБазе = true;
            if (таймерНаСтарт&&!кнопкаПодсвечена)
            {
                if (ускорениеПодсветки) // если стоит режим ускорения подсветки
                {
                    double урезатьНа;
                    if (задержкаПодсветки > данные.среднее)
                    {
                        урезатьНа = задержкаПодсветки * 0.05;
                    }
                    else
                    {
                        урезатьНа = задержкаПодсветки * 0.02;
                    }

                    задержкаПодсветки -= урезатьНа;
                    if (задержкаПодсветки < 10)
                    {
                        задержкаПодсветки = 10;
                    }
                    таймерПодсветки.Interval = задержкаПодсветки; //глюк похоже, иногда при изменении интервала таймер сам запускается что ли, по этому из друго места пришлось сиюда перенести в место прям перед запуском таймера
                }
                таймерПодсветки.Start(); 
            }
            ЗаполнитьЛейбл();
        }
        private void ГлавноеОкно_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            растояниеДокнопки = (grid_Направления2.ActualWidth + grid_Направления2.ActualHeight) / 4 - 20; 
        }


        private void ГлавноеОкно_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (кнопкаПодсвечена) { данные.промах++; ЗаполнитьЛейбл(); }            
        }

        private void checkbox_ОграничениеВремени_Click(object sender, RoutedEventArgs e) { if (checkbox_ОграничениеВремени.IsChecked == true) { ограничениеВремени = true; } else { ограничениеВремени = false; } }
        private void checkbox_УскорениеПодсветки_Click(object sender, RoutedEventArgs e)
        {
            if (checkbox_УскорениеПодсветки.IsChecked == true) { ускорениеПодсветки = true; } else { ускорениеПодсветки = false; }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { slider_реакция.Value = данные.среднее; } //не дает изменять,но isenabled =false не подошло, так как не выводило подсказку при наведении
        #endregion кнопки 


    }
}
