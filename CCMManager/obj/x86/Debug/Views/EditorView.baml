   M S B A M L   `   `   `  ����U  QCaliburn.Micro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8e5891231f2ed21fG4clr-namespace:Caliburn.Micro;assembly=Caliburn.MicroCaliburn.Micro  5      E ACCMManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:CCMManager.ViewsCCMManager.Views 5   '   !   CCMManager.Views.EditorView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Dcal4clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro     5      -myclr-namespace:CCMManager.Views     5   '   
������ .��5      �   � RowDefinitions  5   
   �� 5	      $��30 �6   �� 5
      $��32 �6   �� 5      $��* �6   �� 5      $��32 �6    � ColumnDefinitions 5   
   �� 5      $	��200 �6   �� 5      $��70 �6   �� 5      $��* �6   .+�5   
   � $��0��6   $��0��5      $��3��5      .+�5      �� -   5         ��Name$ 
TitleBlock��5      $��Classroom Editor��5      $��4,4,4,4q�5      $
��Bold"�5      $��14(�5      5      � 5   
   $��1��6   $��0��5      $��3��5      .+�5       ��  �� Orientation$ 
Horizontal=�6   .+�5!      �� $��
Classroom:��6   $��4,4,4,4q�5"      �� 5#      -   5         ��Name$ 
Classrooms��5#      $	��150��5$       �� 
IsEditable .  $��4,4,4,4q�5&       �� DisplayMemberPath$
 Name��5'      �� 5(      -   5         ��Name$ NewRoom��5(      $	��New��5)      $��4,4,4,4q�5*      �� 5+      -   5        $ 
DeleteRoom��5+      $��Delete��5,      $��4,4,4,4q�5-      {�5.      ��  �� ElementName$ 
Classrooms��	 �� Path$	 SelectedItems.Count�5/      50      � 51   
   $��2��6   $��0��52      .+�53      ]� .+�5?      �� -   5        
 ��Name$
 	Computers��5?       � ToolTip$ Current Computers��5@      $��4,4,4,4q�5A      $��0q�5B      $
��Auto=�5C      $
 Name��5D      5E      5F      � 5G   
   $��2��6   $��1��5H      $��Center=�5I      $��Center=�5J      .+�5K      �� $��Center=�6   $��Center=�5L      .+�5M      �� -   5        $ RemoveSelected��5M      $��4,4,4,4q�5N      $ Remove the Selected Item��5O      $��Remove��5Q      {�5P      �� $ 	Computers��$	 SelectedItems.Count�5Q      �� 5R      -   5        $ 	ClearList��5R      $��4,4,4,4q�5S      $ Clear the List��5U      $��Clear��5V      {�5T      �� $ 	Computers��$	 Items.Count�5V      5W      5X      � 5Z   
   $��2��6   $��2��5[      $��Center=�5\      $��Center=�5]      .+�5^      �� .+�5_      �� $��
Add Single��6   $��4q�5`      $��	10,10,4,4q�5a      $��2q�5b      .��5c      �� $ 
Horizontal=�6!   .+�5d      �� $��Computer Name:��6$   $��4,4,4,4q�5e   $   �� 5f      -   5         ��Name$ SingleComputerName��5f   "   $��4,4,4,4q�5g   "   $	��150��5h   "   {�5i   "   �� $ 
Classrooms��$	 Items.Count��� 5j      -	   5        $ AddSingleComputer��5j   !   $��4,4,4,4q�5k   !   $��50��5m   !   $ Add a Single Computer.��5n   !   $	��Add��5o   !   {�5l   !   �� $ SingleComputerName��$	 Text.Length�5o   !   5p      5q      �� 5r      $��Add From Active Directory��6   $��	10,10,4,4q�5s      $��4q�5t      $��2q�5u      .��5v      �� $ 
Horizontal=�6!   .+�5w      �� $��Computer Filter:��6$   $��4,4,4,4q�5x   $   �� 5y      -
   5        $ ADComputerNameFilter��5y   "   $��4,4,4,4q�5z   "   $	��150��5{   "   {�5|   "   �� $ 
Classrooms��$	 Items.Count��� 5}      -   5        $ AddADComputers��5}   !   $��4,4,4,4q�5~   !   $��50��5   !   $9 3Search AD and add computers based on matches found.��5�   !   $	��Add��5�   !   {�5�   !   �� $ ADComputerNameFilter��$	 Text.Length�5�   !   5�      5�      5�      5�      � 5�   
   $��3��6   $��3��5�      $��Right=�5�      .+�5�      �� -   5        $
 Save��5�      $��80��5�      $��5,5,5,5q�5�      $
��Save��5�       �� 	IsDefault . {�5�      �� $ 
Classrooms��$	 Items.Count�5�      5�      5�      