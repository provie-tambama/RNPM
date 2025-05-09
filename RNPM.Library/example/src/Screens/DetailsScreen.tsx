import { Button, StyleSheet, Text, View } from 'react-native'
import React, { useEffect } from 'react'
import {NavigationObserver, fetchWithTimer} from 'react-native-performance-monitor'

const DetailsScreen = () => {
    const [data, setData] = React.useState(null);
    const fetchData = async () => {
        try {
          const response = await fetchWithTimer('D0CB0E3ABAFC643DCE1B0C45CA572B4A','Request 2','https://jsonplaceholder.typicode.com/todos');
          const result = await response.json();
          setData(result);
        } catch (error) {
          console.error('Fetch error:', error);
        }
      };
    const fetchTodo = async () =>{
      try {
        const response = await fetchWithTimer('D0CB0E3ABAFC643DCE1B0C45CA572B4A','Request 1','https://jsonplaceholder.typicode.com/todos/1');
        const result = await response.json();
        setData(result);
      } catch (error) {
        console.error('Fetch error:', error);
      }
    }
    useEffect(() => {
        //console.log('Data:', data);
    }, [data])
  return (
    <View style={styles.container}>
        <NavigationObserver uniqueAccessCode='D0CB0E3ABAFC643DCE1B0C45CA572B4A' />
      <Text style={styles.mainText}>The link below fetches data using the fetchWithTimer utility.</Text>
      <Button title="Fetch Todo 1" onPress={fetchTodo} />
      <Button title="Fetch Todos" onPress={fetchData} />
        <Text>{data ? JSON.stringify(data) : 'No data'}</Text>
    </View>
  )
}

export default DetailsScreen

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
})