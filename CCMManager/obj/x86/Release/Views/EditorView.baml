   M S B A M L   `   `   `  ���� U  QCaliburn.Micro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8e5891231f2ed21fG4clr-namespace:Caliburn.Micro;assembly=Caliburn.MicroCaliburn.Micro  E ACCMManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:CCMManager.ViewsCCMManager.Views !   CCMManager.Views.EditorView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Dcal4clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro     -myclr-namespace:CCMManager.Views     
������ .���   � RowDefinitions  �� $��30 ��� $��32 ��� $��* ��� $��32 � � ColumnDefinitions �� $	��200 ��� $��70 ��� $��* �.+�� $��0��$��0��$��3��.+��� -    ��Name$ 
TitleBlock��$��Classroom Editor��$��4,4,4,4q�$
��Bold"�$��14(�� $��1��$��0��$��3��.+���  �� Orientation$ 
Horizontal=�.+��� $��
Classroom:��$��4,4,4,4q��� -    ��Name$ 
Classrooms��$	��150�� �� 
IsEditable .  $��4,4,4,4q� �� DisplayMemberPath$
 Name���� -    ��Name$ NewRoom��$	��New��$��4,4,4,4q��� -   $ 
DeleteRoom��$��Delete��$��4,4,4,4q�{���  �� ElementName$ 
Classrooms��	 �� Path$	 SelectedItems.Count�� $��2��$��0��.+�]� .+��� -   
 ��Name$
 	Computers�� � ToolTip$ Current Computers��$��4,4,4,4q�$��0q�$
��Auto=�$
 Name��� $��2��$��1��$��Center=�$��Center=�.+��� $��Center=�$��Center=�.+��� -   $ RemoveSelected��$��4,4,4,4q�$ Remove the Selected Item��$��Remove��{��� $ 	Computers��$	 SelectedItems.Count��� -   $ 	ClearList��$��4,4,4,4q�$ Clear the List��$��Clear��{��� $ 	Computers��$	 Items.Count�� $��2��$��2��$��Center=�$��Center=�.+��� .+��� $��
Add Single��$��4q�$��	10,10,4,4q�$��2q�.���� $ 
Horizontal=�.+��� $��Computer Name:��$��4,4,4,4q��� -    ��Name$ SingleComputerName��$��4,4,4,4q�$	��150��{��� $ 
Classrooms��$	 Items.Count��� -	   $ AddSingleComputer��$��4,4,4,4q�$��50��$ Add a Single Computer.��$	��Add��{��� $ SingleComputerName��$	 Text.Length��� $��Add From Active Directory��$��	10,10,4,4q�$��4q�$��2q�.���� $ 
Horizontal=�.+��� $��Computer Filter:��$��4,4,4,4q��� -
   $ ADComputerNameFilter��$��4,4,4,4q�$	��150��{��� $ 
Classrooms��$	 Items.Count��� -   $ AddADComputers��$��4,4,4,4q�$��50��$9 3Search AD and add computers based on matches found.��$	��Add��{��� $ ADComputerNameFilter��$	 Text.Length�� $��3��$��3��$��Right=�.+��� -   $
 Save��$��80��$��5,5,5,5q�$
��Save�� �� 	IsDefault . {��� $ 
Classrooms��$	 Items.Count�