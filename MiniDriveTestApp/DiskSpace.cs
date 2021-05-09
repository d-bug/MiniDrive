using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniDriveTestApp
{
    public class DiskSpace
    {
        public List<DriveModel> ProcessedDrives => processedDrives;

        private List<DriveModel> processedDrives;        
        private List<DriveModel> deletedDrives = new List<DriveModel>();

        /// <summary>
        /// Function to attempt to pack the data onto as few hard drives as possible.
        /// It is assumed that the data consists of very small files,
        /// such that splitting it up and moving parts of it onto different hard drives
        /// never presents a problem.
        /// </summary>
        /// <param name="used">amount of disk space used on each drive</param>
        /// <param name="total">the total capacity of each drive mentioned in used array</param>
        /// <returns>
        ///     The minimum number of hard drives that still contain data after
        /// the consolidation is complete.
        /// </returns>
        public int minDrives(int[] used, int[] total)
        {
            // check input constraints
            if (!ValidateInputData(used, total))
            {
                Console.WriteLine("Input data error! Please check your input data.");
                return -1;
            }

            // store number of drives
            int numberOfDrives = 0;
            
            // debug vars
            int inProcessBytes = 0;
            int outProcessBytes = 0;

            // clear any deleted drives used in calculations
            deletedDrives.Clear();

            // initialize drive info records
            numberOfDrives = used.Length;

            // load data
            processedDrives = LoadData(used, total);

            // debug var
            inProcessBytes = processedDrives.Sum<DriveModel>(x => x.UsedSize);

            // sort drive records by FreeSize (desc)
            processedDrives.Sort(delegate (DriveModel a, DriveModel b)
            {
                return b.FreeSize.CompareTo(a.FreeSize);
            });

            // n-passes (desc)
            for (int n = 0; n < numberOfDrives; n++)
            {
                // n - 1 drive scan
                for (int i = processedDrives.Count - 1; i >= 0; --i)
                {
                    bool found = ProcessIndexedStorage(i);
                }
            }

            // when any any deleted drives found, add them to the processing drives
            // for display and adjust their used size.
            if (deletedDrives.Count > 0)
            {
                processedDrives.AddRange(deletedDrives);

                // Adjust deleted drive usedSize (negative FreeSize => += UsedSize )
                for (int i = 0; i < processedDrives.Count; i++)
                {
                    if (processedDrives[i].FreeSize < 0)
                    {
                        processedDrives[i].UsedSize += processedDrives[i].FreeSize;
                    }
                }
            }

            // re-order by id
            processedDrives.Sort(delegate (DriveModel a, DriveModel b)
            {
                return a.Id.CompareTo(b.Id);
            });

            // debug var
            outProcessBytes = processedDrives.Sum<DriveModel>(x => x.UsedSize);
            if (inProcessBytes != outProcessBytes)
            {
                Console.WriteLine($"ERROR [in - out] = {inProcessBytes} - {outProcessBytes}");
            }

            // return drives still containing data after consolidation
            int retVal = processedDrives.Count<DriveModel>(x => x.UsedSize > 0);

            // return the min drives calculated.
            return (retVal);
        }

        /// <summary>
        /// Helper function to convert input data into model data
        /// </summary>
        /// <param name="used"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<DriveModel> LoadData(int[] used, int[] total)
        {
            int arraySize = total.Length;

            List<DriveModel> drives = new List<DriveModel>(arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                DriveModel model = new DriveModel()
                {
                    Id = i + 1,
                    Label = $"Drive {i + 1}",
                    TotalSize = total[i],
                    UsedSize = used[i]
                };

                drives.Add(model);
            }

            return drives;
        }

        /// <summary>
        /// Data input validation function
        /// </summary>
        /// <param name="used"></param>
        /// <param name="total"></param>
        /// <returns>true if all input data requirements are met, false otherwise.</returns>
        private bool ValidateInputData(int[] used, int[] total)
        {
            if (used.Length != total.Length)
            {
                Console.WriteLine("Both [used] input array and [total] input array should have identical sizes.");
                return false;
            }

            if (used.Length < 1 || used.Length > 50 || total.Length < 1 || total.Length > 50)
            {
                Console.WriteLine("Both [used] input array and [total] input array should contain up to 50 elements.");
                return false;
            }

            for (int i = 0; i < used.Length; i++)
            {
                if (used[i] > total[i])
                {
                    Console.WriteLine("Every element in [used] array must be <= to the corresponding one in the [total] array");
                    return false;
                }
            }

            for (int i = 0; i < total.Length; i++)
            {
                if (total[i] < 1 || total[i] > 1000)
                {
                    Console.WriteLine("Every element in [total] array must be between 1 and 1000 inclusive");
                    return false;
                }
            }

            for (int i = 0; i < used.Length; i++)
            {
                if (used[i] < 1 || used[i] > 1000)
                {
                    Console.WriteLine("Every element in the [used] array must be between 1 and 1000 inclusive");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds and moves data from drives other than the one defined by index
        /// </summary>
        /// <param name="index">Dive index to move data to</param>
        /// <returns>true if any movement is possible, false otherwise</returns>
        private bool ProcessIndexedStorage(int index)
        {
            // cache drive count
            int count = processedDrives.Count;

            // check index validation
            if (index < 0 || index >= count)
            {
                return false;
            }

            // the storage amount we are seeking
            int requiredSize = processedDrives[index].FreeSize;

            // scan all (not full) drives except indexer
            // to find proper storage availability
            bool found = false;
            for (int i = count - 1; i >= 0; --i)
            {
                // dont test against ourselves!
                if (i == index)
                    continue;

                // not enough space to trade found in drive
                if (processedDrives[i].UsedSize == 0)
                    continue;

                // do not update storage info if no free storage exists
                if (index < processedDrives.Count && processedDrives[index].FreeSize > 0)
                {

                    // clamp required size to the max slot can give
                    requiredSize = Math.Min(processedDrives[i].UsedSize, requiredSize);

                    // swap required sizes between drives (index = caller)
                    processedDrives[i].UsedSize -= requiredSize;
                    processedDrives[index].UsedSize += requiredSize;
                    found = true;

                    // if current processed drive's freesize <= 0,
                    // exclude it from valid processing drives
                    if (processedDrives[i].FreeSize <= 0)
                    {
                        deletedDrives.Add(processedDrives[i]);

                        // remove it from the processing list
                        processedDrives.RemoveAt(i);
                    }

                }
            }

            return found;
        }

    }

}
