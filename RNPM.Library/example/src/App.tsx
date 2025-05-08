import React, { useEffect } from 'react'
import { StyleSheet } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import HomeScreen from './Screens/HomeScreen';
import ProfileScreen from './Screens/ProfileScreen';
import type { RootStackParamList } from './types';
import DetailsScreen from './Screens/DetailsScreen';
import { getSavedMetrics } from '../../src/utils/offlineStorage';
import { initRetryMechanism } from '../../src/utils/retryMetrics';

const Stack = createNativeStackNavigator<RootStackParamList>();

const App = () => {
useEffect(() => {
  getSavedMetrics().then((data) => {
    console.log('Saved metrics:', data);
    
  });
  initRetryMechanism();
}, [])
  return (
    <NavigationContainer>
      <Stack.Navigator initialRouteName="Home">
      <Stack.Screen name="Home">
          {props => <HomeScreen {...props} uniqueAccessCode="925BEBE09CAD13641792961A5DEC7962" />}
        </Stack.Screen>
        <Stack.Screen name="Profile" component={ProfileScreen} />
        <Stack.Screen name="Details" component={DetailsScreen} />
      </Stack.Navigator>
    </NavigationContainer>
  );
};

export default App;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
});