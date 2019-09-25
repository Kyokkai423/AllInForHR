using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace AllInOutForHR
{
    public class WindowClosingBehavior : Behavior<UsersList>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Closing += AssociatedObject_Closing;
        }
        /// <summary>
        /// Анимация открытия/закрытия окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_Closing(object sender, CancelEventArgs e)
        {
            UsersList window = sender as UsersList;
            window.Closing -= AssociatedObject_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.5));
            anim.Completed += (s, _) => window.Close();
            window.BeginAnimation(UIElement.OpacityProperty, anim);
        }
        protected override void OnDetaching()
        {
            AssociatedObject.Closing -= AssociatedObject_Closing;
        }
    }
}
