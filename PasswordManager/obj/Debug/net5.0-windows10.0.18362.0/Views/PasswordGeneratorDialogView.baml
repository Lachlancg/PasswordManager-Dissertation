   M S B A M L   `   `   `  ����P  LPasswordManagerClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:PasswordManagerClient.ViewsPasswordManagerClient.Views  5      =    7PasswordManagerClient.Views.PasswordGeneratorDialogView   5      \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:PasswordManagerClient.Views     5      ���#C5C6C7.��5
      �   � RowDefinitions  5   
   �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6     � ColumnDefinitions 5      �� 5      $
��Auto �6#   .+�5   
   ]� $��0��6   $��10q�6!   .+�5      �� $��Right=�6k    �� TextAlignment$ Right=�6�   $��40��6�   ��6   ��  �� ElementName$ Slider�� �� Path$ Value� �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5      -   5        $��Slider��5   .    �� TickFrequency$ 1Q�6<    �� IsSnapToTickEnabled$
 True��6N    � Maximum$ 50Q�6i   $	��407��6v   $��35��6�   	 � Value	 6   �� *Slider+6�   5      �� 5   
   $��10q�6   
 �� Orientation$
 
Horizontal=�6"   $��1��6;   .+�5      ��  � ToolTip$ 
Lower Case��6   $��18��6L    d� 	IsChecked 6,   �� *	LowerCase+.��5      �� $��
Lower Case��6   60   5      �� 5      $ 
Upper Case��6    6,   �� *	UpperCase+.��5      �� $��
Upper Case��6   60   5       �� 5!      $ Numbers��6    6)   �� *	Numbers+.��5"      �� $��Numbers��6   6-   5#      �� 5$      $ Special Characters��6    64   �� *SpecialChars+.��5%      �� $��Special Chars��6   63   5&      5'      �� 5)   
   $��2��6   .+�5*      �� $��10q�69   ��6   �� *GeneratedPassword+6G   � 5+      -   5        $��PasswordScore��5+   `    � Minimum$ -1Q�6v   $ 5Q�6�   $	��407��6�   $��35��6�   ��6   �� *PasswordColour+	 6@   �� *PasswordScore+6�   5-      ]� 50   
   $��3��6   .+�51      �� $
��Left=�6�   $��10q�6�   $��32��6�   $	��220��6�   ��6   ��  �� RelativeSource ��  �� AncestorType
 Window��6f   �� *CancelCommand+.��6�   Cancel6�   �� 52      $��Right=�6�   $��10q�6�   $��32��6�   $	��240��6�   ��6   ��  �� 
 Window��6f   �� *CopyCommand+.��6�   Copy6�   53      55      56      