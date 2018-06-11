namespace Efour
{
    public class TrainingLabModule : PartModule
    {
        ProtoCrewMember[] crewArr = new ProtoCrewMember[8];
        string[] eventArr = 
        {
            "TrainKerbalInside0",
            "TrainKerbalInside1",
            "TrainKerbalInside2",
            "TrainKerbalInside3",
            "TrainKerbalInside4",
            "TrainKerbalInside5",
            "TrainKerbalInside6",
            "TrainKerbalInside7",
        };

        string[] trainingArr =
        {
            "",
            "Training1",
            "Training2",
            "Training3",
            "Training4",
            "Training5"
        };

        float[] levelUpExpTable = { 2, 6, 8, 16, 32, 0 };

        string[] levelNumber = { "1st", "2nd", "3rd", "4th", "5th", "null"};

        [KSPField]
        public int TrainFactor = 20;

        [KSPField]
        public float inSpace = 0.5f;

        [KSPField]
        public float Landed = 0.25f;

        [KSPField(guiActive = false, guiName = "Sci")]
        public int SciRemain;

        [KSPEvent(guiActive = false, guiName = "Training0")]
        public void TrainKerbalInside0()
        {
            TrainKerbal(0);
        }
        [KSPEvent(guiActive = false, guiName = "Training1")]
        public void TrainKerbalInside1()
        {
            TrainKerbal(1);
        }
        [KSPEvent(guiActive = false, guiName = "Training2")]
        public void TrainKerbalInside2()
        {
            TrainKerbal(2);
        }
        [KSPEvent(guiActive = false, guiName = "Training3")]
        public void TrainKerbalInside3()
        {
            TrainKerbal(3);
        }
        [KSPEvent(guiActive = false, guiName = "Training4")]
        public void TrainKerbalInside4()
        {
            TrainKerbal(4);
        }
        [KSPEvent(guiActive = false, guiName = "Training5")]
        public void TrainKerbalInside5()
        {
            TrainKerbal(5);
        }
        [KSPEvent(guiActive = false, guiName = "Training6")]
        public void TrainKerbalInside6()
        {
            TrainKerbal(6);
        }
        [KSPEvent(guiActive = false, guiName = "Training7")]
        public void TrainKerbalInside7()
        {
            TrainKerbal(7);
        }

        private float calculateSciCost(float baseValue)
        {
            float ret = baseValue * TrainFactor;

            if (this.vessel.mainBody.bodyName == "Kerbin" && this.vessel.LandedOrSplashed) return ret;
            else if (this.vessel.LandedOrSplashed) return ret * Landed;
            else return ret * inSpace;
        }

        private void TrainKerbal(int index)
        {
            ProtoCrewMember crew = crewArr[index];

            int lastLog = 0;
            FlightLog totalLog = crew.careerLog.CreateCopy();
            totalLog.MergeWith(crew.flightLog.CreateCopy());
            foreach(FlightLog.Entry entry in totalLog.Entries)
            {
                if (lastLog < 1 && entry.type == "Training1") lastLog = 1;
                if (lastLog < 2 && entry.type == "Training2") lastLog = 2;
                if (lastLog < 3 && entry.type == "Training3") lastLog = 3;
                if (lastLog < 4 && entry.type == "Training4") lastLog = 4;
                if (lastLog < 5 && entry.type == "Training5") lastLog = 5;
            }

            if (lastLog == 5)
            {
                ScreenMessages.PostScreenMessage(crew.name + " already had every trainings.");
                return;
            }

            float SciCost = calculateSciCost(levelUpExpTable[lastLog]);
            if (ResearchAndDevelopment.Instance.Science < SciCost)
            {
                ScreenMessages.PostScreenMessage("Insufficient Science Point.\n" + 
                    "Needed : " + SciCost + ", Remain : " + ResearchAndDevelopment.Instance.Science);
                return;
            }
            ResearchAndDevelopment.Instance.AddScience(-1 * SciCost, TransactionReasons.CrewRecruited);
            crew.flightLog.AddEntry(new FlightLog.Entry(crew.flightLog.Flight, trainingArr[lastLog+1], "Kerbin"));
            ScreenMessages.PostScreenMessage(levelNumber[lastLog] + " Training Complete : " + crew.name);

        }

        public override void OnUpdate()
        {
            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER) return;

            Fields["SciRemain"].guiActive = true;
            SciRemain = (int) ResearchAndDevelopment.Instance.Science;

            int index = 0;
            for (int cnt = 0; cnt < crewArr.Length; cnt++) crewArr[cnt] = null;
            foreach (ProtoCrewMember crew in part.protoModuleCrew)
            {
                if (index >= 8) break;
                int lastLog = 0;
                FlightLog totalLog = crew.careerLog.CreateCopy();
                totalLog.MergeWith(crew.flightLog.CreateCopy());
                foreach (FlightLog.Entry entry in totalLog.Entries)
                {
                    if (lastLog < 1 && entry.type == "Training1") lastLog = 1;
                    if (lastLog < 2 && entry.type == "Training2") lastLog = 2;
                    if (lastLog < 3 && entry.type == "Training3") lastLog = 3;
                    if (lastLog < 4 && entry.type == "Training4") lastLog = 4;
                    if (lastLog < 5 && entry.type == "Training5") lastLog = 5;
                }

                crewArr[index] = crew;
                int SciCost = (int) calculateSciCost(levelUpExpTable[lastLog]);

                if (lastLog < 5) Events[eventArr[index]].guiName = "[" + lastLog + "->" + (lastLog + 1) + "] " + crew.name + "[" + SciCost + "p]";
                else Events[eventArr[index]].guiName = "[5]" + crew.name;

                Events[eventArr[index]].guiActive = true;
                index++;
            }

            for(; index < eventArr.Length; index++) Events[eventArr[index]].guiActive = false;
        }
    }
}
