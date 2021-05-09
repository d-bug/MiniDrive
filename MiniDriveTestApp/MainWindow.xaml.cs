using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniDriveTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int[,] TestData1 = new int[2, 3] { { 300, 525, 110 }, { 350, 600, 115 } };
        private readonly int[,] TestData2 = new int[2, 6] { { 1, 200, 200, 199, 200, 200 }, { 1000, 200, 200, 200, 200, 200 } };
        private readonly int[,] TestData3 = new int[2, 5] { { 750, 800, 850, 900, 950 }, { 800, 850, 900, 950, 1000 } };
        private readonly int[,] TestData4 = new int[2, 50] { { 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49 }, { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 } };
        private readonly int[,] TestData5 = new int[2, 20] { { 331, 242, 384, 366, 428, 114, 145, 89, 381, 170, 329, 190, 482, 246, 2, 38, 220, 290, 402, 385 }, { 992, 509, 997, 946, 976, 873, 771, 565, 693, 714, 755, 878, 897, 789, 969, 727, 765, 521, 961, 906 } };

        // Drives model
        public List<DriveModel> drives = new List<DriveModel>();
        // selected data set

        private int[,] currentData = null;

        // flag to indicate that the consolidation process is active
        private bool bIsProcessed = false;

        public MainWindow()
        {
            InitializeComponent();

        }
    
        /// <summary>
        /// Display the drive data information as entered
        /// </summary>
        /// <param name="data"></param>
        private void DisplayDrives(int[,] data)
        {
            // fetch data
            int[] used = GetRowData(data, 0);
            int[] total = GetRowData(data, 1);

            // convert data into model
            DiskSpace diskSpace = new DiskSpace();

            // load data in the model
            drives = diskSpace.LoadData(used, total);

            // add all model rows to the data grid
            PopulateDriveData();

            txtResult.Text = "Press [Process] to attempt to pack the data onto as few hard drives as possible";
        }

        /// <summary>
        /// Try to find the minimum number of drives needed
        /// </summary>
        /// <param name="data"></param>
        private void ProcessDrives(int[,] data)
        {
            DiskSpace diskSpace = new DiskSpace();

            // fetch data
            int[] used = GetRowData(data, 0);
            int[] total = GetRowData(data, 1);

            // convert
            int retVal = diskSpace.minDrives(used, total);

            // display results
            if (retVal != -1)
            {
                drives = diskSpace.ProcessedDrives;

                PopulateDriveData();

                txtResult.Text = $" {retVal } hard drive(s) still contain data after the consolidation is complete.";
            }
        }

        /// <summary>
        /// add model records to the data grid
        /// </summary>
        private void PopulateDriveData()
        {
            DriveDataGrid.Items.Clear();

            foreach (var d in drives)
            {
                DriveDataGrid.Items.Add(d);
            }
        }

        /// <summary>
        /// returns a dimension from the passed data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private static int[] GetRowData(int[,] data, int row)
        {
            int index = data.GetLength(1);

            return Enumerable.Range(0, index)
                .Select(x => data[row, x])
                .ToArray();
        }

        /// <summary>
        /// process button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            if (!bIsProcessed && currentData != null)
            {
                bIsProcessed = true;
                ProcessDrives(currentData);

                Button button = (Button)sender;
                button.IsEnabled = false;
            }
        }

        /// <summary>
        /// Tests combobox changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbTests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTests.SelectedIndex != -1)
            {
                switch (cbTests.SelectedIndex)
                {
                    case 0:
                        currentData = TestData1;
                    break;

                    case 1:
                        currentData = TestData2;
                    break;

                    case 2:
                        currentData = TestData3;
                        break;

                    case 3:
                        currentData = TestData4;
                        break;
                    
                    case 4:
                        currentData = TestData5;
                        break;
                }

                if (currentData != null)
                {
                    DisplayDrives(currentData);

                    btnProcess.IsEnabled = true;

                    bIsProcessed = false;
                }
            }
        }
    }

}
