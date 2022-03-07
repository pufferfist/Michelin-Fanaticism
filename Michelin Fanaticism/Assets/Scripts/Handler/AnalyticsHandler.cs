using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class AnalyticsHandler
    {
        private LevelConfig levelConfig;
        
        public AnalyticsHandler(LevelConfig levelConfig)
        {
            this.levelConfig = levelConfig;
        }

        public void upload(int score, Dictionary<string, int> recipePopularity,int barrierEncounter,Dictionary<int, float> trackPopularity,bool success
        )
        {
            Dictionary<string, object> statistics = new Dictionary<string, object>
            {
                {"level", levelConfig.Level},
                {"score", score},
                {"success",success},
                {"barrierEncountered", barrierEncounter},
                {"leftTrackTime", trackPopularity[0]},
                {"middleTrackTime", trackPopularity[1]},
                {"rightTrackTime", trackPopularity[2]}
            };
            AnalyticsResult scoreAnalytics = Analytics.CustomEvent("Statistics",statistics);
            Debug.Log("score analytics: "+scoreAnalytics);
            foreach (KeyValuePair<string,object> statistic in statistics)
            {
                Debug.Log(statistic.Key+": "+statistic.Value);
            }
            

            Dictionary<string, object> popularityResult = new Dictionary<string, object>();
            recipePopularity.ToList().ForEach(x => popularityResult.Add(x.Key, x.Value));
            AnalyticsResult popularityAnalytics = Analytics.CustomEvent("Recipe Popularity",
                popularityResult);
            Debug.Log("popularity analytics: "+popularityAnalytics);
        }
    }
}