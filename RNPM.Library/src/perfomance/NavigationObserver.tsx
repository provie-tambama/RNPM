import { useNavigation } from '@react-navigation/native';
import React, { useEffect } from 'react';
import { getNavigationData } from '../utils/navigationTimer';
import { createNavigationMetric } from '../utils/api';

type NavigationObserverProps = {
  uniqueAccessCode: string;
  onNavigationTimeMeasured?: (time: number, from: string, to: string) => void;
};

const NavigationObserver: React.FC<NavigationObserverProps> = ({ uniqueAccessCode, onNavigationTimeMeasured }) => {
  const navigation = useNavigation();

  useEffect(() => {
    const handleFocus = () => {
      const data = getNavigationData();
      if (data !== null && data !== undefined) {
        const { startTime, targetScreen, fromScreen } = data;
        const endTime = performance.now();
        const navigationTime = endTime - startTime;
        console.log(`Navigation time: ${navigationTime}ms`);
        console.log(`Navigating from ${fromScreen} to ${targetScreen} with unique access code ${uniqueAccessCode}`);
        if (onNavigationTimeMeasured) {
          onNavigationTimeMeasured(navigationTime, fromScreen, targetScreen);
        }
        const send = async () => {
        
          await createNavigationMetric(uniqueAccessCode, fromScreen, targetScreen, navigationTime);
        }
        send();
      }
    };

    const unsubscribeFocus = navigation.addListener('focus', handleFocus);

    return () => {
      unsubscribeFocus();
    };
  }, [navigation, onNavigationTimeMeasured]);

  return null;
};

export default NavigationObserver;
