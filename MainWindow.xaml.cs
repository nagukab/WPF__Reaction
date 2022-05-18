using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
    public partial class MainWindow : Window
    {
        double растояниеДокнопки, лимитВремени = 2000, задержкаПодсветки = 1000;
        bool таймерНаСтарт = false, кнопкаПодсвечена = false, ограничениеВремени = false, ускорениеПодсветки = false,НаБазе=false, звукОшибки;
        DateTime текущееВРемя;
        TimeSpan интервал;
        Данные данныеМышкой = new Данные(), данныеКнопкой = new Данные();
        Timer таймерПлохойРеакции = new Timer(), таймерПодсветки = new Timer();
        Random рандом = new Random();
        Brush стандартныйЦвет;
        Key искатьКнопку;
        Button измененнаяКнопка;

        public MainWindow()
        {
            InitializeComponent();            
            KeyDown += MainWindow_KeyDown;  //  KeyDown += (q, e) => кнопка_ОднаИз4ех_Click(q, e);
            звукОшибки = Convert.ToBoolean(checkbox_ЗвукОшибки.IsChecked);
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
                case 1: ИзменитьКнопку(кнопка_Верх); искатьКнопку = Key.Up; измененнаяКнопка = кнопка_Верх; break;
                case 2: ИзменитьКнопку(кнопка_Низ); искатьКнопку = Key.Down; измененнаяКнопка = кнопка_Низ; break;
                case 3: ИзменитьКнопку(кнопка_Лево); искатьКнопку = Key.Left; измененнаяКнопка = кнопка_Лево; break;
                case 4: ИзменитьКнопку(кнопка_Право); искатьКнопку = Key.Right; измененнаяКнопка = кнопка_Право; break;
            }
            кнопкаПодсвечена = true;
            таймерПодсветки.Stop();

            if (ограничениеВремени) // если включен режим ограничения времени
            {
                double урезатьНа = (лимитВремени - ((данныеМышкой.среднее==0? данныеКнопкой.среднее: данныеМышкой.среднее) +(данныеКнопкой.среднее==0? данныеМышкой.среднее : данныеКнопкой.среднее))/2) * 0.2; // урезать лимит на 20% от интервала между лимитом и средней задержкой
                лимитВремени -= урезатьНа;
                таймерПлохойРеакции.Interval = лимитВремени; //глюк похоже, иногда при изменении интервала таймер сам запускается что ли
                таймерПлохойРеакции.Start();
            }
            ЗаполнитьЛейбл();
        }

        private void ТаймерПлохойРеакции_Elapsed(object sender, ElapsedEventArgs e)
        {
            История($"{DateTime.Now:HH:mm:ss_fff} ОПАЗДАНИЕ - прошло {таймерПлохойРеакции.Interval} мс"); таймерПлохойРеакции.Stop(); ЗаполнитьЛейбл();
            ЗвукНеудачи();
        }

        #endregion события

        #region  ____________________________________МЕТОДЫ

        private void MainWindow_KeyDown(object sender, KeyEventArgs клавиша)
        {
            if (клавиша.Key == искатьКнопку)
            {
                if (ограничениеВремени)
                {
                    if (таймерПлохойРеакции.Enabled) // если таймер плохой реакции ещё идет, значит нажато вовремя
                    {
                        таймерПлохойРеакции.Stop(); данныеКнопкой.вовремя++;
                    }
                    else { данныеКнопкой.опаздание++; }
                }
                кнопкаПодсвечена = false;
                измененнаяКнопка.Background = стандартныйЦвет;
                интервал = DateTime.Now.Subtract(текущееВРемя);
                double дабл = интервал.TotalMilliseconds;
                данныеКнопкой.листРеакций.Add(дабл);
                if (данныеКнопкой.листРеакций.Count > 0)
                {
                    данныеКнопкой.среднее = данныеКнопкой.листРеакций.Average();
                    данныеКнопкой.максимальное = данныеКнопкой.листРеакций.Max();
                    данныеКнопкой.минимальное = данныеКнопкой.листРеакций.Min();
                    данныеКнопкой.попаданийВсего = данныеКнопкой.листРеакций.Count();
                    Dispatcher.Invoke(() => { slider_реакция_Кнопкой.Value = данныеКнопкой.листРеакций.Average(); slider_реакция_Кнопкой.ToolTip = $"среднее время реакции Кнопкой {данныеКнопкой.листРеакций.Average()} мс"; });
                }
                Перезапуск(); искатьКнопку = default; измененнаяКнопка = null;
                История($"{DateTime.Now:HH:mm:ss_fff} реакция Кнопкой последняя={дабл}   средняя={данныеКнопкой.среднее}   миниум={данныеКнопкой.минимальное}   максимум={данныеКнопкой.максимальное}");
            }
            else
            {
                История($"{DateTime.Now:HH:mm:ss_fff} ОШИБКА - нажатие не на ту {клавиша.Key} кнопку"); данныеКнопкой.ошибок++; ЗвукНеудачи();
            }
            ЗаполнитьЛейбл();
        }
        void НажатиеНаКнопку(object кнопка, RoutedEventArgs чемНажато)
        {
            Button даКнопка = кнопка as Button;
            if (кнопкаПодсвечена && даКнопка?.Background == Brushes.Red)
            {
                if (ограничениеВремени)
                {
                    if (таймерПлохойРеакции.Enabled) // если таймер плохой реакции ещё идет или ограничение отключено, значит нажато вовремя
                    {
                        таймерПлохойРеакции.Stop(); данныеМышкой.вовремя++;
                    }
                    else { данныеМышкой.опаздание++; }
                }
                кнопкаПодсвечена = false;
                даКнопка.Background = стандартныйЦвет;
                интервал = DateTime.Now.Subtract(текущееВРемя);
                double дабл = интервал.TotalMilliseconds;
                данныеМышкой.листРеакций.Add(дабл);
                if (данныеМышкой.листРеакций.Count > 0)
                {
                    данныеМышкой.среднее = данныеМышкой.листРеакций.Average();
                    данныеМышкой.максимальное = данныеМышкой.листРеакций.Max();
                    данныеМышкой.минимальное = данныеМышкой.листРеакций.Min();
                    данныеМышкой.попаданийВсего = данныеМышкой.листРеакций.Count();
                    Dispatcher.Invoke(() => { slider_реакция_Мышкой.Value = данныеМышкой.листРеакций.Average(); slider_реакция_Мышкой.ToolTip = $"среднее время реакции Мышкой {данныеМышкой.листРеакций.Average()} мс"; });
                }

                История($"{DateTime.Now:HH:mm:ss_fff} реакция Мышкой последняя={дабл}   средняя={данныеМышкой.среднее}   миниум={данныеМышкой.минимальное}   максимум={данныеМышкой.максимальное}");
                искатьКнопку = default; измененнаяКнопка = null;
            }
            else if (кнопкаПодсвечена && даКнопка.Background != Brushes.Red) { История($"{DateTime.Now:HH:mm:ss_fff} ОШИБКА - клик не на ту кнопку"); данныеМышкой.ошибок++; ЗвукНеудачи(); }
            ЗаполнитьЛейбл();
        }

        void ЗаполнитьЛейбл()
        {
            Dispatcher.Invoke(() =>
            {
                label_инфо.Content = $"                 мышкой           кнопкой\n" +
                        $"max                     {данныеМышкой.максимальное}            {данныеКнопкой.максимальное}\n" +
                        $"среднее                 {данныеМышкой.среднее}            {данныеКнопкой.среднее}\n" +
                        $"min                     {данныеМышкой.минимальное}            {данныеКнопкой.минимальное}\n" +
                        $"попаданийВсего          {данныеМышкой.попаданийВсего}            {данныеКнопкой.попаданийВсего} \n" +
                        $"вовремя                 {данныеМышкой.вовремя}            {данныеКнопкой.вовремя}\n" +
                        $"опазданий               {данныеМышкой.опаздание}            {данныеКнопкой.опаздание}\n" +                        
                        $"ошибок                  {данныеМышкой.ошибок}            {данныеКнопкой.ошибок}  \n" +
                        $"промахов Мышкой         {данныеМышкой.промах}        \n" +
                        $"среднееРастояниеДоКнопки={растояниеДокнопки}\n" +
                        $"лимитВремени={лимитВремени}   задержкаПодсветки={задержкаПодсветки}\n" +
                        $"набазе{НаБазе}";                
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

        void ЗвукНеудачи() { if (звукОшибки) { using (MemoryStream звук = new MemoryStream(Properties.Resources.Errorwav)) new SoundPlayer(звук).Play(); } }

        void Перезапуск()
        {
            if (таймерНаСтарт && !кнопкаПодсвечена)
            {
                if (ускорениеПодсветки) // если стоит режим ускорения подсветки
                {
                    double урезатьНа;
                    if (задержкаПодсветки > (данныеМышкой.среднее+данныеКнопкой.среднее)/2) { урезатьНа = задержкаПодсветки * 0.05; }
                    else { урезатьНа = задержкаПодсветки * 0.02; }
                    задержкаПодсветки -= урезатьНа;

                    if (задержкаПодсветки < 10) { задержкаПодсветки = 10; }
                }
                if (НаБазе) {таймерПодсветки.Start(); таймерПодсветки.Interval = задержкаПодсветки; }//глюк похоже, иногда при изменении интервала таймер сам запускается что ли, по этому из друго места пришлось сиюда перенести в место прям перед запуском таймера
                
            }
            ЗаполнитьЛейбл();
        }
        void Цвета() { if (таймерНаСтарт) { кнопка_Старт.Background = стандартныйЦвет; кнопка_Стоп.Background = Brushes.DeepSkyBlue; } else { кнопка_Старт.Background = Brushes.Green; кнопка_Стоп.Background = стандартныйЦвет; } }
        #endregion методы


        #region  ____________________________________КНОПКИ

        private void кнопка_Старт_Click(object sender, RoutedEventArgs e) { таймерНаСтарт = true; кнопка_База.Background = Brushes.Green; textbox_история.Clear(); Цвета(); }
        private void кнопка_Стоп_Click(object sender, RoutedEventArgs e) { таймерНаСтарт = false; таймерПодсветки.Stop(); Цвета(); }
        private void кнопка_ОднаИз4ех_Click(object sender, RoutedEventArgs even) { НажатиеНаКнопку(sender, even); }
        private void кнопка_Сброс_Click(object sender, RoutedEventArgs e)
        {
            таймерПодсветки.Stop(); таймерПлохойРеакции.Stop(); таймерНаСтарт = false;
            slider_реакция_Кнопкой.Value = 0; slider_реакция_Мышкой.Value = 0;
            данныеМышкой = new Данные(); данныеКнопкой = new Данные();
            textbox_история.Clear();
            ЗаполнитьЛейбл(); таймерПлохойРеакции.Interval = лимитВремени; Цвета();
        }
        private void кнопка_База_MouseLeave(object sender, MouseEventArgs e)
        {
            кнопка_База.Background = Brushes.Yellow;
            НаБазе = false;
            if (!кнопкаПодсвечена)//если курсор убирается до того как кнопк подсветится, то останавливать таймер
            {
                таймерПодсветки.Stop();
            }
            ЗаполнитьЛейбл();
        }
        private void кнопка_База_MouseEnter(object sender, MouseEventArgs e)
        {
            кнопка_База.Background = стандартныйЦвет;
            НаБазе = true;
            Перезапуск();
        }

        private void ГлавноеОкно_SizeChanged_1(object sender, SizeChangedEventArgs e) { растояниеДокнопки = (grid_Направления2.ActualWidth + grid_Направления2.ActualHeight) / 4 - 20; }


        private void ГлавноеОкно_MouseDown(object sender, MouseButtonEventArgs e)
        { if (кнопкаПодсвечена) { данныеМышкой.промах++; История($"{DateTime.Now:HH:mm:ss_fff} ПРОМАХ - клик мимо кнопки"); ЗаполнитьЛейбл(); ЗвукНеудачи(); } }

        private void checkbox_ОграничениеВремени_Click(object sender, RoutedEventArgs e) { if (checkbox_ОграничениеВремени.IsChecked == true) { ограничениеВремени = true; } else { ограничениеВремени = false; } }
        private void checkbox_УскорениеПодсветки_Click(object sender, RoutedEventArgs e) { if (checkbox_УскорениеПодсветки.IsChecked == true) { ускорениеПодсветки = true; } else { ускорениеПодсветки = false; } }
        private void checkbox_ЗвукОшибки_Click(object sender, RoutedEventArgs e) { if (checkbox_ЗвукОшибки.IsChecked == true) { звукОшибки = true; } else { звукОшибки = false; } }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { slider_реакция_Мышкой.Value = данныеМышкой.среднее; slider_реакция_Кнопкой.Value = данныеКнопкой.среднее; } //не дает изменять,но isenabled =false не подошло, так как не выводило подсказку при наведении
        #endregion кнопки
    }
}
