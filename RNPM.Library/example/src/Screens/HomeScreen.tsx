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

  const testRequest = async () => {
    fetch('http://192.168.100.3:7018/api/apiManagement/ping')
  .then(response => {
    // Check if the request was successful
    console.log('Response status:', response.text);
    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }
    // Parse the JSON response
    console.log('Response JSON:', response.json);
    return response.json();
  })
  .then(data => {
    // Do something with the data
    console.log('ping:', data);
    return data; // Return the data for further use if needed
  })
  .catch(error => {
    // Handle any errors that occurred during the fetch
    console.error('Fetch error:', error);
    console.log('Error message:', error.message);
    console.log("error.stack:", error.stack);
  });
  }

  return (
    <ScrollView style={{backgroundColor:"#fff"}}>
          <View style={styles.container}>
      <Text style={styles.mainText}>Screen to test the render metrics measurement</Text>
      <Text style={{textAlign:"center", marginTop:50, marginHorizontal:10}}>To test <Text style={{fontWeight:"800"}}>Navigations</Text>, you can navigate to the Details Screen and the Profile Screen</Text>
      <Text style={{textAlign:"center", marginTop:15, marginHorizontal:10, marginBottom:30}}>To test <Text style={{fontWeight:"800"}}>Network Request Completion Time</Text>, you can navigate to the Details Screen</Text>
      <Button  title="Go to Details" onPress={handleGoToDetails} />
      <Button title="Go to Profile" onPress={handleGoToProfile} />
      <Button title="Test Network Request" onPress={testRequest} />
    </View>
    </ScrollView>

  );
};

var code = HomeScreen.toString();
console.log(code);

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
