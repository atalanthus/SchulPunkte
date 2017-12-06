﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SchulPunkte;
using System.Diagnostics;

namespace SchulPunkteUI
{
    /// <summary>
    /// Interaction logic for KurseEinstellen.xaml
    /// </summary>
    public partial class KurseEinstellen : Window
    {
        #region Attribute
        private KursListBoxItem SelectedKurs;
        private Style ListBoxItemStyle;

        private Manager Manager;
        private enum KursnamenTeile { name, nummer };
        #endregion

        #region Konstruktor
        public KurseEinstellen()
        {
            InitializeComponent();
            ListBoxItemStyle = this.FindResource("defaultListBoxItem") as Style;

            Manager = Manager.Instance;
            SelectedKurs = null;

            foreach (Kurs kurs in Manager.Kurse)
            {
                KursListBoxItem kursItem = new KursListBoxItem(kurs, kurs.GetKursInfo());
                //kursItem.Style = ListBoxItemStyle;
                KurseListBox.Items.Add(kursItem);
            }

            KursnameFeedback.Content = "";
            KursnummerFeedback.Content = "";
        }
        #endregion

        #region Methoden
        /// <summary>
        /// Ueberprueft, ob ein Kursname in der aktuellen Liste bereits vorkommt.
        /// </summary>
        /// <param name="neuerKursname">Kursname, der ueberprueft werden soll.</param>
        /// <returns>false, wenn der Kursname bereits vorkommt, ansonsten false</returns>
        private bool KursnamePruefen(KursListBoxItem neuerKurs)
        {
            string neuerKursname = neuerKurs.Kurs.Kursname;

            if (neuerKurs.Kurs.Kursname.ToLower() != "kursname")
            {
                foreach (KursListBoxItem kursItem in KurseListBox.Items)
                {
                    if (kursItem.Kurs.Kursname.ToLower() == neuerKursname.ToLower() && kursItem != neuerKurs)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Ueberprueft, ob eine Kursnummer in der aktuellen Liste bereits vorkommt.
        /// </summary>
        /// <param name="neueKursnummer">Kursnummer, die ueberprueft werden soll.</param>
        /// <returns>false, wenn die Kursnummer bereits vorkommt, ansonsten false</returns>
        private bool KursnummerPruefen(KursListBoxItem neuerKurs)
        {
            string neueKursnummer = neuerKurs.Kurs.Kursnummer;

            if (neuerKurs.Kurs.Kursnummer.ToLower() != "kursnummer")
            {
                foreach (KursListBoxItem kursItem in KurseListBox.Items)
                {
                    if (kursItem.Kurs.Kursnummer.ToLower() == neueKursnummer.ToLower() && kursItem != neuerKurs)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Ueberprueft, ob ein Kurs Objekt in der Liste einen Fehler hat (Fehler ist true)
        /// und enabled oder disabled je nachdem den 'WeiterBtn'.
        /// </summary>
        private void FehlerPruefen()
        {
            bool listeEnthaeltFehler = false;
            foreach (KursListBoxItem kursItem in KurseListBox.Items)
            {
                if (kursItem.Fehler)
                {
                    kursItem.Foreground = Brushes.Red;
                    listeEnthaeltFehler = true;
                }
            }

            if (listeEnthaeltFehler)
                WeiterBtn.IsEnabled = false;
            else
                WeiterBtn.IsEnabled = true;
        }
        #endregion

        #region Event Handler
        /// <summary>
        /// Click Event fuer 'Neuen Kurs hinzufuegen' Button
        /// Fuegt neuen Kurs mit Standart Werten hinzu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KursHinzufuegen(object sender, RoutedEventArgs e)
        {
            KursListBoxItem leererKurs = new KursListBoxItem(new Kurs("Kursname", "Kursnummer"), "Kursname (Kursnummer)");
            KurseListBox.Items.Add(leererKurs);
            KurseListBox.SelectedItem = leererKurs;
            SelectedKurs = leererKurs;
        }

        /// <summary>
        /// SelectionChanged Event fuer Kurs Liste
        /// Aktualisiert die 'Kurs bearbeiten' Inputs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KurseListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectedKurs = (KursListBoxItem)KurseListBox.SelectedValue;

            if (SelectedKurs == null)
                return;

            NeuerKursname.Text = SelectedKurs.Kurs.Kursname;
            NeueKursnummer.Text = SelectedKurs.Kurs.Kursnummer;
        }

        /// <summary>
        /// TextChanged Event fuer 'NeuerKursname' Input
        /// Ueberprueft, den neuen Kursnamen fuer den aktuell 
        /// ausgewaehlten Kurs. Passt etwas nicht, wird eine Fehlermeldung unter dem
        /// Input Feld angezeigt, ansonsten uebernimmt das ausgewaehlte 
        /// Kurs Objekt in der Kurs Liste die Kursname und Kursnummer Werte, der
        /// 'Kurs bearbeiten' Form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NeuerKursname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedKurs == null)
                return;

            SelectedKurs.Kurs.Kursname = NeuerKursname.Text;

            if (NeuerKursname.Text.Contains("("))
            {
                KursnameFeedback.Content = "Ungültiges Zeichen: (";
                SelectedKurs.Fehler = true;
            }
            else if (!KursnamePruefen(SelectedKurs))
            {
                KursnameFeedback.Content = "Dieser Kursname existiert bereits!";
                SelectedKurs.Fehler = true;
            }
            else
                KursnameFeedback.Content = "";

            FehlerPruefen();
            
            SelectedKurs.Content = NeuerKursname.Text + " (" + NeueKursnummer.Text + ")";
        }

        /// <summary>
        /// TextChanged Event fuer 'NeueKursnummer' Input
        /// Ueberprueft, die neue Kursnummer fuer den aktuell 
        /// ausgewaehlten Kurs. Passt etwas nicht, wird eine Fehlermeldung unter dem
        /// Input Feld angezeigt, ansonsten uebernimmt das ausgewaehlte 
        /// Kurs Objekt in der Kurs Liste die Kursname und Kursnummer Werte, der
        /// 'Kurs bearbeiten' Form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NeueKursnummer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedKurs == null)
                return;

            SelectedKurs.Kurs.Kursnummer = NeueKursnummer.Text;

            if (NeueKursnummer.Text.Contains("("))
            {
                KursnummerFeedback.Content = "Ungültiges Zeichen: (";
                SelectedKurs.Fehler = true;
            }
            else if (!KursnummerPruefen(SelectedKurs))
            {
                KursnummerFeedback.Content = "Diese Kursnummer existiert bereits!";
                SelectedKurs.Fehler = true;
            }
            else
                KursnummerFeedback.Content = "";

            FehlerPruefen();
            
            SelectedKurs.Content = NeuerKursname.Text + " (" + NeueKursnummer.Text + ")";
        }

        private void NeuerKursname_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Tab))
                NeuerKursname.SelectAll();
        }

        private void NeueKursnummer_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Tab))
                NeueKursnummer.SelectAll();
        }

        private void Help(object sender, RoutedEventArgs e)
        {
            //TODO: Hilfe Panel anzeigen
        }

        private void WeiterBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.Kurse.Clear();
            foreach (KursListBoxItem kursItem in KurseListBox.Items)
            {
                if (kursItem.Kurs.Kursname.ToLower() != "kursname" && kursItem.Kurs.Kursnummer.ToLower() != "kursnummer")
                    Manager.Kurse.Add(kursItem.Kurs);
            }

            Close();
        }

        //TODO: Kurse löschen können (nach Bestaetigung)
        //Moechtest du wirklich den Kurs #kurs mit n eingetragenen Leistungserhebungen loeschen?
        #endregion
    }
}