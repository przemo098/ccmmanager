   M S B A M L   `   `   `  ����U  QCaliburn.Micro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8e5891231f2ed21fG4clr-namespace:Caliburn.Micro;assembly=Caliburn.MicroCaliburn.Micro  5   	    
CCMManager    CCMManager.Views.ShellView   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Dcal4clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro     5   	   $	��600��5   	   $	��800��5   	      WindowBrush#��[  5   	     � Icon$5  //CCMManager;component/Apps-system-diagnosis.ico��5   	   .��5	      � $��
10,10,10,2q�6    � RowDefinitions 5
   
   �� 5      $��40 �6   �� 5      $��* �6   �� 5      $��24 �6    � ColumnDefinitions 5   
   �� 5      $��* �6   '  !System.Windows.VisualStateManager   VisualStateGroups 5   
   %  System.Windows.VisualStateGroup  5      -   5        AppTitleStates��   Transitions 5      %  System.Windows.VisualTransition  5         GeneratedDuration$ 0:0:0:4D�6'      States. 5         System.Windows.VisualState  -   5        
Normal��   
Storyboard. 5      �� .��5      W�  g� Duration$ 0D�6*   		 W� To$
	 0.35X�5   *   
 �� TargetProperty$
 (UIElement.Opacity)�5   *    �� 
TargetName$ DisplayName��5   *   W� 5      $ 0D�6*   $
	 0.35X�5   *   $
 (UIElement.Opacity)�5    *   $ 	rectangle��5!   *   5"      5#        5$      -   5        	MouseOver��. 5%      �� .��5&      W� $ 0D�6*   $
 (UIElement.Opacity)�5'   *   $ 	rectangle��5(   *   W� 5)      $ 0D�6*   $
	 0.75X�5*   *   $
 (UIElement.Opacity)�5+   *   $ DisplayName��5,   *   5.      5/      50      .+�54   
   �� -   5         ��Name$ Header��54      ���	#FF2D3539$
��18pt(�56      $��Italic$�57      $��Verdana)�58      $��	0,0,100,0q�59      $��0��5:       J� BitmapEffect 5;      O� 5<       O� 	GlowColor$ White��6(   �� 5>      $��CCM Classroom Manager��6   5?      �� 5A   
   $��0,5,5,0q�6   $��Right=�5B      $	��Top=�5C      $��2��5D      $��0��5E      ��5F      �� 5G       �� 
TargetType" ��6"   .R�5H      �� $ ��/Resources/Images/Info.png��6   $��48��5I   $   $��48��5J   $    � ToolTip$ Click here for Info��5K   $    � Cursor$
 Hand��5L   $    J� Opacity$	 0.8Q�5M   $   5N      5P      ]� 5U   
   $��1��6   .+�5V      �� -   5         ��Name$ 
ActiveItem��5V      $��Stretch=�5W      $��Stretch=�5X      5Y      �� 5_   
   $��2��6   
������ .@�5a      �� $��StatusBarText��6   5b      �� 5g   
   -   5         ��Name$ Shadow��5g      $��2��5h      $��4��5i      
���   �$��-10,-10,-10,-2q�5k      $
 0.36Q�5l      $y�	Collapsed=�5m      5o      5p      