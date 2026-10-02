using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLETestApp.Trainer
{
    public class TrainerState
    {
        public bool IsConnected { get; set; } 
        public double SpeedKph { get; set; }    // Instantaneous speed
        public double AverageSpeedKph { get; set; } 
        public double CadenceRpm { get; set; }  // Instantaneous cadence
        public double AverageCadenceRpm { get; set; }
        public double DistanceKm { get; set; }
        public double ResistanceLevel { get; set; }
        public int PowerWatts { get; set; }     // Instantaneous power
        public int AveragePowerWatts { get; set; }
        public int TotalEnergyKcal { get; set; }
        public int HeartRate { get; set; } 
        public double MetabolicEquivalent { get; set; }
        public int ElapsedTimeSeconds { get; set; }
        public int RemainingTimeSeconds { get; set; }



        public TrainerState()
        {
            IsConnected = false;
            SpeedKph = 0;
            AverageSpeedKph = 0;
            CadenceRpm = 0;
            AverageCadenceRpm = 0;
            PowerWatts = 0;
            AveragePowerWatts = 0;
            DistanceKm = 0;
            HeartRate = 0;
            ResistanceLevel = 0;
            TotalEnergyKcal = 0;
            MetabolicEquivalent = 0;
            ElapsedTimeSeconds = 0;
            RemainingTimeSeconds = 0;
        }

        public TrainerState(bool isConnected, 
                            double speed, double avgSpeed, 
                            double cadence, double avgCadence, 
                            int power, int avgPower, 
                            double distance,
                            int heartRate, 
                            double resistanceLevel,
                            int energy
                            ) 
        {
            IsConnected = isConnected;

            SpeedKph = speed;
            AverageSpeedKph = avgSpeed;

            CadenceRpm = cadence;
            AverageCadenceRpm = avgCadence;

            PowerWatts = power;
            AveragePowerWatts = avgPower;

            DistanceKm = distance;

            HeartRate = heartRate;

            ResistanceLevel = resistanceLevel;

            TotalEnergyKcal = energy;
        }

        public string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}]", IsConnected, SpeedKph, AverageSpeedKph, CadenceRpm, AverageCadenceRpm, PowerWatts, AveragePowerWatts, DistanceKm, HeartRate, ResistanceLevel, TotalEnergyKcal);
        }
    }
}
