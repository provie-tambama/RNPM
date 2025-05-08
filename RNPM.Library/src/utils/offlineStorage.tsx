import AsyncStorage from '@react-native-async-storage/async-storage';

const METRICS_KEY = 'offlineMetrics';

export const saveMetric = async (metric: object) => {
  try {
    const metrics = await AsyncStorage.getItem(METRICS_KEY);
    const metricsArray = metrics ? JSON.parse(metrics) : [];
    metricsArray.push(metric);
    await AsyncStorage.setItem(METRICS_KEY, JSON.stringify(metricsArray));
  } catch (error) {
    console.log('Error saving metric:', error);
  }
};

export const getMetrics = async () => {
  try {
    const metrics = await AsyncStorage.getItem(METRICS_KEY);
    return metrics ? JSON.parse(metrics) : [];
  } catch (error) {
    console.log('Error retrieving metrics:', error);
    return [];
  }
};

export const getSavedMetrics = async () => {
  try {
    const metrics = await AsyncStorage.getItem(METRICS_KEY);
    return metrics ? JSON.parse(metrics) : [];
  } catch (error) {
    console.log('Error retrieving metrics:', error);
    return [];
  }
};

export const removeMetrics = async () => {
  try {
    await AsyncStorage.removeItem(METRICS_KEY);
  } catch (error) {
    console.log('Error removing metrics:', error);
  }
};
