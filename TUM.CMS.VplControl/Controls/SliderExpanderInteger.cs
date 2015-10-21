using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TUM.CMS.VplControl.Controls
{

    public class SliderExpanderInteger : Expander
    {
        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register("SliderValue",
                typeof(int), typeof(SliderExpanderInteger)); // optionally metadata for defaults etc.

        public int SliderValue
        {
            get { return (int)GetValue(SliderValueProperty); }
            set { SetValue(SliderValueProperty, value); }
        }


        public static readonly DependencyProperty SliderMinProperty =
            DependencyProperty.Register("SliderMin",
                typeof(int), typeof(SliderExpanderInteger)); // optionally metadata for defaults etc.

        public int SliderMin
        {
            get { return (int)GetValue(SliderMinProperty); }
            set { SetValue(SliderMinProperty, value); }
        }

        public static readonly DependencyProperty SliderMaxProperty =
            DependencyProperty.Register("SliderMax",
                typeof(int), typeof(SliderExpanderInteger)); // optionally metadata for defaults etc.

        public int SliderMax
        {
            get { return (int)GetValue(SliderMaxProperty); }
            set { SetValue(SliderMaxProperty, value); }
        }

        public static readonly DependencyProperty SliderStepProperty =
    DependencyProperty.Register("SliderStep",
        typeof(int), typeof(SliderExpanderInteger)); // optionally metadata for defaults etc.

        public int SliderStep
        {
            get { return (int)GetValue(SliderStepProperty); }
            set { SetValue(SliderStepProperty, value); }
        }
    }

}
