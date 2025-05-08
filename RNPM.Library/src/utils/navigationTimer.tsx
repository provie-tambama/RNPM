type NavigationData = {
    startTime: number;
    fromScreen: string;
    targetScreen: string;
  };
  
  let navigationData: NavigationData | null = null;
  
  export const setNavigationStartTime = (time: number, fromScreen: string, targetScreen: string) => {
    navigationData = { startTime: time, targetScreen, fromScreen };
  };
  
  export const getNavigationData = (): NavigationData | null => {
    return navigationData;
  };
  