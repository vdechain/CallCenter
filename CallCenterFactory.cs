namespace CallCenter
{
    internal static class CallCenterFactory
    {
        public static CallCenter Create(int nbWorkers, int nbManager) {
            return new CallCenter(nbWorkers, nbManager);
        }

    }
}
