import * as React from 'react';
import { StyleSheet, View, Text, Button } from 'react-native';
import { NavigationObserver, setNavigationStartTime } from 'react-native-performance-monitor';
import { useNavigation } from '@react-navigation/native';
import type { ProfileScreenNavigationProp } from '../types';

function ProfileScreen() {
  const navigation = useNavigation<ProfileScreenNavigationProp>();

  const handleGoToHome = () => {
    setNavigationStartTime(performance.now(), 'Profile', 'Details');
    navigation.navigate('Details');
  };

  const delay = (ms: number) => {
    const end = performance.now() + ms;
    while (performance.now() < end) {
      
    }
  };

  delay(1000); // 1000 milliseconds delay

  return (
    <View style={styles.container}>
      <NavigationObserver uniqueAccessCode='925BEBE09CAD13641792961A5DEC7962'/>
      <Text style={styles.mainText}>This screen has a forced delay to measure navigation time when some processes run before the component renders</Text>
      <Button title="Go to Home" onPress={handleGoToHome} />
    </View>
  );
}

export default ProfileScreen;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  box: {
    width: 60,
    height: 60,
    marginVertical: 20,
  },
  mainText:{
    fontSize:20,
    textAlign:"center"
  }
});
