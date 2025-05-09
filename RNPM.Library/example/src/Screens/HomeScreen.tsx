import React from 'react';
import { View, Text, Button, StyleSheet, ScrollView} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { type HomeScreenNavigationProp } from '../types';
import { setNavigationStartTime, withRenderTimeMonitor } from 'react-native-performance-monitor';
import { registerComponentSource } from 'react-native-performance-monitor';

const HomeScreen: React.FC<{ uniqueAccessCode: string }> = ({ uniqueAccessCode }) => { 'show source'
  const navigation = useNavigation<HomeScreenNavigationProp>();

  const handleGoToDetails = () => {
    setNavigationStartTime(performance.now(),'Home', 'Details');
    navigation.navigate('Details');
  };

  const handleGoToProfile = () => {
    setNavigationStartTime(performance.now(),'Home', 'Profile');
    navigation.navigate('Profile');
  };

  return (
    <ScrollView style={{backgroundColor:"#fff"}}>
          <View style={styles.container}>
      <Text style={styles.mainText}>Screen to test the render metrics measurement</Text>
      <Text style={{textAlign:"center", marginTop:50, marginHorizontal:10}}>To test <Text style={{fontWeight:"800"}}>Navigations</Text>, you can navigate to the Details Screen and the Profile Screen</Text>
      <Text style={{textAlign:"center", marginTop:15, marginHorizontal:10, marginBottom:30}}>To test <Text style={{fontWeight:"800"}}>Network Request Completion Time</Text>, you can navigate to the Details Screen</Text>
      <Button  title="Go to Details" onPress={handleGoToDetails} />
      <Button title="Go to Profile" onPress={handleGoToProfile} />
    </View>
    </ScrollView>

  );
};

export default withRenderTimeMonitor(HomeScreen);

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    paddingTop:50
  },
  mainText:{
    fontSize:20,
    textAlign:"center"
  }
});
