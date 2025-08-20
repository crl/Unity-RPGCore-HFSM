namespace RPGCore.AI.HFSM
{
    public static class StringUtil
    {
        public static string ToTitleCase(string s)
        {
            return s.Substring(0,1).ToUpper()+s.Substring(1); 
        }
    }
}