using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Medvedeva_PrivateAdertisementService
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }
        private Users currentUser;
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginBox.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }
            try
            {
                using (var db = new Entities())
                {
                    var user = db.Users
                        .AsNoTracking()
                        .FirstOrDefault(u => u.Login == LoginBox.Text && u.PasswordHash == PasswordBox.Password);

                    if (user == null)
                    {
                        MessageBox.Show("Пользователь с такими данными не найден!");
                    }
                    else
                    {
                        MessageBox.Show("Пользователь успешно найден!");
                        (Application.Current.MainWindow as MainWindow)?.MainFrame.Navigate(new UserAdsPage(user.UserID));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка доступа к базе данных: {ex.Message}");
            }
        }
        
    private void Guest_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.MainFrame.Navigate(new GuestViewPage());
        }
    }
}