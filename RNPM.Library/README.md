# @provie17/react-native-performance-monitor

A comprehensive solution for monitoring and optimizing the performance of React Native applications in both development and production environments. Track component render times, navigation latency, and network request durations with AI-powered optimization suggestions.

## Installation

```sh
npm install @provie17/react-native-performance-monitor
```

## Usage

### 1. Create an account and get your unique access code

Visit the web dashboard to create an account and register your application to receive a unique access code.

### 2. Component Render Time Monitoring

Wrap your components with the `withRenderTimeMonitor` higher-order component:

```js
import React from 'react';
import { View, Text } from 'react-native';
import { withRenderTimeMonitor } from '@provie17/react-native-performance-monitor';

const MyComponent = () => {
  return (
    <View>
      <Text>Hello World!</Text>
    </View>
  );
};

// Wrap component with performance monitoring
export default withRenderTimeMonitor(MyComponent);
```

### 3. Navigation Performance Monitoring

Add the `NavigationObserver` to destination screens and use `setNavigationStartTime` in start screens:

```js
// In your destination screen
import React from 'react';
import { View, Text } from 'react-native';
import { NavigationObserver } from '@provie17/react-native-performance-monitor';

const DestinationScreen = () => {
  return (
    <View>
      {/* Add the NavigationObserver with your unique access code */}
      <NavigationObserver uniqueAccessCode="YOUR_UNIQUE_ACCESS_CODE" />
      <Text>Destination Screen</Text>
    </View>
  );
};

export default DestinationScreen;

// In your starting screen
import { useNavigation } from '@react-navigation/native';
import { setNavigationStartTime } from '@provie17/react-native-performance-monitor';

const StartScreen = () => {
  const navigation = useNavigation();

  const handleNavigation = () => {
    // Record navigation start time before navigating
    setNavigationStartTime(performance.now(), 'StartScreen', 'DestinationScreen');
    navigation.navigate('DestinationScreen');
  };

  // Component JSX...
};
```

### 4. Network Request Monitoring

Use `fetchWithTimer` instead of the standard fetch API:

```js
import { fetchWithTimer } from '@provie17/react-native-performance-monitor';

const fetchData = async () => {
  try {
    const response = await fetchWithTimer(
      'YOUR_UNIQUE_ACCESS_CODE',
      'RequestName', // Descriptive name for this request
      'https://api.example.com/data'
    );
    
    const data = await response.json();
    // Process your data
  } catch (error) {
    console.error('Error fetching data:', error);
  }
};
```

### 5. Component Source Registration

Register component sources in your main App component to enable code optimization suggestions:

```js
import React, { useEffect } from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { registerComponentSource } from '@provie17/react-native-performance-monitor';

const App = () => {
  useEffect(() => {
  }, []);

  return (
    <NavigationContainer>
      {/* Your app components */}
    </NavigationContainer>
  );
};

export default App;
```

## Features

- 📊 **Accurate Performance Metrics**: Precise measurements of component render times, navigation delays, and network requests
- 🔍 **Production Monitoring**: Track performance in real-world usage scenarios
- 📱 **Cross-device Support**: Works across different device categories with minimal overhead
- 🧠 **AI-Powered Optimization**: Receive actionable optimization suggestions through ML analysis
- 📈 **Interactive Dashboard**: Visualize performance metrics through a web interface

## Contributing

See the [contributing guide](CONTRIBUTING.md) to learn how to contribute to the repository and the development workflow.

## License

MIT

---

Made with [create-react-native-library](https://github.com/callstack/react-native-builder-bob)