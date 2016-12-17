using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EntryPoint
{
#if WINDOWS || LINUX
    public static class Program
    {

        [STAThread]
        static void Main()
        {

            var fullscreen = false;
            read_input:
            switch (
                Microsoft.VisualBasic.Interaction.InputBox(
                    "Which assignment shall run next? (1, 2, 3, 4, or q for quit)", "Choose assignment",
                    VirtualCity.GetInitialValue()))
            {
                case "1":
                    using (var game = VirtualCity.RunAssignment1(SortSpecialBuildingsByDistance, fullscreen))
                        game.Run();
                    break;
                case "2":
                    using (
                        var game = VirtualCity.RunAssignment2(FindSpecialBuildingsWithinDistanceFromHouse, fullscreen))
                        game.Run();
                    break;
                case "3":
                    using (var game = VirtualCity.RunAssignment3(FindRoute, fullscreen))
                        game.Run();
                    break;
                case "4":
                    using (var game = VirtualCity.RunAssignment4(FindRoutesToAll, fullscreen))
                        game.Run();
                    break;
                case "q":
                    return;
            }
            goto read_input;
        }

        private static Vector2 globalHouse { get; set; }
        private static Vector2[] globalSpecialBuildings { get; set; }

        private static IEnumerable<Vector2> SortSpecialBuildingsByDistance(Vector2 house,
            IEnumerable<Vector2> specialBuildings)
        {
            globalHouse = house;
            globalSpecialBuildings = specialBuildings.ToArray();
            MergeSort(globalSpecialBuildings, 0, globalSpecialBuildings.Length - 1);
            return globalSpecialBuildings;
            //return specialBuildings.OrderBy(v => Vector2.Distance(v, house));
        }

        private static void MergeSort(Vector2[] specialBuildings, int left, int right)
        {
            if (right > left)
            {
                int mid = (right + left)/2;
                MergeSort(specialBuildings, left, mid);
                MergeSort(specialBuildings, (mid + 1), right);
                Merge(specialBuildings, left, mid, right);
            }
        }

        private static void Merge(Vector2[] specialBuildings, int left, int mid, int right)
        {
            int i, j, k;
            int n1 = mid - left + 1;
            int n2 = right - mid;
            Vector2[] tempLeft = new Vector2[n1 + 1];
            Vector2[] tempRight = new Vector2[n2 + 1];
            for (i = 0; i < n1; i++)
            {
                tempLeft[i] = specialBuildings[left + i];
            }

            for (j = 0; j < n2; j++)
            {
                tempRight[j] = specialBuildings[mid + j + 1];
            }

            i = 0;
            j = 0;
            k = left;
            for (k = left; k <= right; k++)
            {
                if (CalcDistanceOfHouse(tempLeft[i]) <= CalcDistanceOfHouse(tempRight[j]))
                {
                    specialBuildings[k] = tempLeft[i];
                    i++;
                }
                else
                {
                    specialBuildings[k] = tempRight[j];
                    j++;
                }
            }
            globalSpecialBuildings = specialBuildings;
        }

        private static double CalcDistanceOfHouse(Vector2 specialBuilding)
        {
            var distance = Math.Sqrt(Math.Pow((globalHouse.X - specialBuilding.X), 2) + Math.Pow((globalHouse.Y - specialBuilding.Y), 2));
            
            return distance;
        }

        private static IEnumerable<IEnumerable<Vector2>> FindSpecialBuildingsWithinDistanceFromHouse(
          IEnumerable<Vector2> specialBuildings,
          IEnumerable<Tuple<Vector2, float>> housesAndDistances)
        {
            return
                from h in housesAndDistances
                select
                  from s in specialBuildings
                  where Vector2.Distance(h.Item1, s) <= h.Item2
                  select s;
        }

        private static IEnumerable<Tuple<Vector2, Vector2>> FindRoute(Vector2 startingBuilding,
          Vector2 destinationBuilding, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
            var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
            List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
            var prevRoad = startingRoad;
            for (int i = 0; i < 30; i++)
            {
                prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, destinationBuilding)).First());
                fakeBestPath.Add(prevRoad);
            }
            return fakeBestPath;
        }

        private static IEnumerable<IEnumerable<Tuple<Vector2, Vector2>>> FindRoutesToAll(Vector2 startingBuilding,
          IEnumerable<Vector2> destinationBuildings, IEnumerable<Tuple<Vector2, Vector2>> roads)
        {
            List<List<Tuple<Vector2, Vector2>>> result = new List<List<Tuple<Vector2, Vector2>>>();
            foreach (var d in destinationBuildings)
            {
                var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
                List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
                var prevRoad = startingRoad;
                for (int i = 0; i < 30; i++)
                {
                    prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, d)).First());
                    fakeBestPath.Add(prevRoad);
                }
                result.Add(fakeBestPath);
            }
            return result;
        }
    }
#endif
}
