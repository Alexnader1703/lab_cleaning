   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.MainWindow   5   	   localclr-namespace:WpfApp1  5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Chemical Cleaning��5   	   $	��600��6#   $	��800��60    � 	Resources 5      %   WpfApp1.IndexToBooleanConverter   IndexToBoolean(   "WpfApp1.IndexToVisibilityConverter  IndexToVisibility'   !WpfApp1.ObjectToTypeNameConverter  ObjectToTypeNameConverter��%W   &	        &	      &	 &       5   
   6(     5   
   6+     5	   
   6*   5
      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   ]� $��0��6   $��10q�6!   .+�5      ��  �� Orientation$ 
Horizontal=�6   $
��Left=�62   .+�5      	� $��Clients��6   $��0,0,10,0q�6�    d� 	IsChecked 60   �� *SelectedTab+ �� 	Converter# [   �� ConverterParameter$ 0��6�   	� 5      $��Services��6   $��0,0,10,0q�6�    61   �� *SelectedTab+# [  $ 1��6�   	� 5      $��Orders��6   $��0,0,10,0q�6�    6/   �� *SelectedTab+# [  $ 2��6�   	� 5      $��Receipts��6   $��0,0,10,0q�6�    61   �� *SelectedTab+# [  $ 3��6�   5      �� 5      $ 
Horizontal=�6   $��Right=�62   .+�5      �� -   5        $��+��5      $��0,0,10,0q�6%   67   �� 5      -   5        $��Сохранить��5      $��0,0,10,0q�6-   6?   �� 5      -   5        $��Print Receipt��5      61   5      5      � 5!   
   $��1��6   $��10q�6   .+�5"      &   System.Windows.Controls.DataGrid  -   5          Name$ dataGrid��5"         AutoGenerateColumns$ False��6)   	   
IsReadOnly$	 False��6i   
   CanUserAddRows$
 False��6|      CanUserDeleteRows$
 True��6�   ��6E   �� *CurrentData+6�   5$      5%      5&      