using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace de.ahzf.Vanaheimr.Aegir.Controls
{

    /// <summary>
    /// Interaction logic for InfoBox.xaml
    /// </summary>
    public partial class InfoBox : UserControl
    {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(String), typeof(InfoBox));
        public static readonly DependencyProperty BodyProperty  = DependencyProperty.Register("Body",  typeof(String), typeof(InfoBox));

        public String Title
        {

            get
            {
                return (String) GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }

        }

        public String Body
        {
            
            get
            {
                return (String) GetValue(BodyProperty);
            }

            set
            {
                SetValue(BodyProperty, value);
            }

        }

        public InfoBox()
        {

            InitializeComponent();

            this.DataContext = this;

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {

        }



    }

}
