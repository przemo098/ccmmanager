   M S B A M L   `   `   `  ���� U  QCaliburn.Micro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8e5891231f2ed21fG4clr-namespace:Caliburn.Micro;assembly=Caliburn.MicroCaliburn.Micro   
CCMManager&    CCMManager.Views.ActionsHomeView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Ccm4clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro       � 	Resources     
LinkButton �� 
TargetType �� Property �� Value �� 
TargetType��%^   &	        �� " ��.���  � �� �� " ��.R��� �� ��  �@�Cursor$
 Hand��.��]� .+��� $��Right=�$��4,1,1,1q�.+�9�  9� ExpandDirection$
 Down=� 9� 
IsExpanded . $��0,1,1,1q�
�������$��2,15,2,0q�$��2q����� " 9� �� Triggers ��  �� Binding �� 	 �� ElementName$	 
Classrooms��
 �� Path$
 Items.Count� �� Value 0.a���  � y�$ 	Collapsed=����� $��12(�$
��Bold"���  �� Orientation$ 
Horizontal=�.+��� ���� *SelectedClassroom.Name+�� $�� Actions��.���� .+��� $��10,4,4,4q�.@��� .+��� -    ��Name$ DDRScanClassroom��$��SCCM Data Discovery Cycle��#��[  �� -   $ PolicyDownloadClassroom��$��SCCM Policy Retrieval��#��[  �� -   $ PolicyApplyClassroom��$��SCCM Policy Evaluation��#��[  �� -   $ PolicyResetClassroom��$��SCCM Policy Reset��#��[  �� -   $ PolicyHardResetClassroom��$��SCCM Policy Hard Reset��#��[  �� -   $ DCMScanClassroom��$��SCCM DCM Scan��#��[  �� -   $ RebootClassroom��$��Reboot��#��[  �� -   $ ShutdownClassroom��$��Shutdown��#��[  �� -	   $ LogoffClassroom��$��Force Log Off��#��[  �� -
   $ LockScreenClassroom��$��Lock Screen��#��[  $y�	Collapsed=��� -   $ GPUpdateClassroom��$��GPUpdate Force��#��[  �� -   $ WakeClassroom��$��Send WOL��#��[  9� $
 Down=� . $��0,1,1,1q�
�������$��1,15,1,0q�$��2q����� " 9� ��  �� $	 
SelectedPC��$
 Text.Length� 0.a���  � y�$ 	Collapsed=����� $��12(�$
��Bold"��� $ 
Horizontal=�.+��� -    ��Name$ 
SelectedPC������ *SelectedComputer.Name+�� $�� Actions��.���� $��10,4,4,4q�.@��� .+��� -   $ DDRScanSelected��$��SCCM Data Discovery Cycle��#��[  �� -   $ PolicyDownloadSelected��$��SCCM Policy Retrieval��#��[  �� -   $ PolicyApplySelected��$��SCCM Policy Evaluation��#��[  �� -   $ PolicyResetSelected��$��SCCM Policy Reset��#��[  �� -   $ PolicyHardResetSelected��$��SCCM Policy Hard Reset��#��[  �� -   $ DCMScanSelected��$��SCCM DCM Scan��#��[  �� -   $ RebootSelected��$��Reboot��#��[  �� -   $ ShutdownSelected��$��Shutdown��#��[  �� -   $ LogOffSelected��$��Force Log Off��#��[  �� -   $ LockSelected��$��Lock Screen��#��[  $y�	Collapsed=��� -   $ GPUpdateSelected��$��GPUpdate Force��#��[  �� -   $ WakeSelected��$��Send WOL��#��[  �� $ 
Horizontal=�$	��Top=�.+��� -    ��Name$ 
Classrooms��$	��120��$��5,4,0,4q� �� DisplayMemberPath$
 Name���� -   $ EditClassrooms��$
��Auto��
������ $��5,4,0,4q� � ToolTip$ Edit Classrooms.������ $'��!/Resources/Images/folder_edit.png��$��16��$��16��d� -    d�Name$ togStatusPoller��$��Status Poller��$��5,4,5,4q��� -    ��Name$ 	Computers��$��4,2,4,4q�$
��Auto=�$	��419�� �� SelectionMode$ Single=� �� View �� .P���  �� Width$ 50��$��Status�� �� CellTemplate �� .R���  ��Name$ StatusImage��$��16�� �� *Status+���� *CurrentStatusImage+ �� Triggers �� $ LoggedOn�� �� *Status+.a���  �� 
TargetName$ StatusImage�� �� *LoggedOnUser+ �@�ToolTip�� $	 100��$
��Name�� �� DisplayMemberBinding �� *Name+�� $	 200��$��ActionStatus�� �� .R��� $ 
Horizontal=�.+��� ���� *	Actions+���� .R��� $ 
Horizontal=����� .R��� $ 
Horizontal=�.+��� $ ActionStatus��$��16������ *CurrentActionImage+ �� $ 
Horizontal=�.+��� ���� *RemoteActionName+�� $	�� - ���� ���� *State+