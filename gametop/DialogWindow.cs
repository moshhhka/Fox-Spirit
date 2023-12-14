using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace gametop
{
    public partial class DialogWindow : Window
        {
            public DialogWindow()
            {
                this.Height = 400;
                this.Width = 600;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Создание текстового блока для вопроса
                TextBlock question = new TextBlock();
                question.Text = "Как дела?";
                question.Margin = new Thickness(10);

                // Создание кнопок для ответа
                Button button1 = new Button() { Content = "Хорошо", Margin = new Thickness(10) };
                button1.Click += (sender, e) => { MessageBox.Show("Хорошо"); };

                Button button2 = new Button() { Content = "Плохо", Margin = new Thickness(10) };
                button2.Click += (sender, e) => { MessageBox.Show("Плохо"); };

                Button button3 = new Button() { Content = "Пока", Margin = new Thickness(10) };
                button3.Click += (sender, e) => { this.Close(); };

                // Добавление элементов на окно
                StackPanel stackPanel = new StackPanel();
                stackPanel.Children.Add(question);
                stackPanel.Children.Add(button1);
                stackPanel.Children.Add(button2);
                stackPanel.Children.Add(button3);
                this.Content = stackPanel;
            }
        }
    }
