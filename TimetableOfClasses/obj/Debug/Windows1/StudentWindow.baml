   M S B A M L   `   `   `  ����M  ITimetableOfClasses, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:TimetableOfClasses.Windows1TimetableOfClasses.Windows1  5   	   /    )TimetableOfClasses.Windows1.StudentWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ;local)clr-namespace:TimetableOfClasses.Windows1     5   	   -   5        $	��305��5   	   $	��301��6     � 
ResizeMode$  NoResize=�5
   	   .��5      � $��0,0,0,0q�6   .+�5   
   �� $��0,0,0,0q�6   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��0, 10, 0, 0q�62   .+�5      �� .��6   Фамилия6!   �� 5      -   5         ��Name$ SurnameTextBox��5         TextBox#��[  62   $	��195��6S   $��15,0,0,0q�6_   5      �� 5      $ 
Horizontal=�6   $��0, 10, 0, 0q�62   .+�5      �� .��6   Имя6   �� 5      -   5        $ NameTextBox��5      #��[  6/   $	��195��6P   $��42,0,0,0q�6\   5      �� 5      $ 
Horizontal=�6   $��0, 10, 0, 0q�62   .+�5      �� .��6   Отчество6"   �� 5      -   5        $ PatronymicTextBox��5      #��[  65   $	��195��6V   $��14,0,0,0q�6b   5      �� 5      $ 
Horizontal=�6   $��0,10,0,0q�62   .+�5      �� .��6   Группа6    �� 5      -   5         ��Name$ GroupComboBox��5       �� DisplayMemberPath$
 Name��62   $	��195��6K   $��27,0,0,0q�6W   5      5      �� 5      $ 
Horizontal=�6   $��0, 10, 0, 0q�62   .+�5      �� .��6   
Логин6   �� 5       -   5        $ LoginTextBox��5       #��[  60   $	��195��6Q   $��31,0,0,0q�6]   5!      �� 5"      $ 
Horizontal=�6   $��0, 10, 0, 0q�62   .+�5#      �� .��6   Пароль6    �� 5$      -   5        $ PasswordTextBox��5$      #��[  63   $	��195��6T   $��24,0,0,0q�6`   5%      �� 5&      -   5         ��Name$ 
SaveButton��5&      $��Сохранить��6)    
 Button#��[ 6>   $��114,10,96,0q�6^   $
��Auto��6�   5'      5(      5)      