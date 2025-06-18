import {apiUrl} from '../constants/api';
import { saveMetric } from './offlineStorage';
import { getDeviceInfoForMetrics } from './deviceInfo';


export const createComponentRenderMetric = async (uniqueAccessCode: string, name: string, renderTime: number) => {
  //console.log("body", uniqueAccessCode, name, renderTime);
  const url = `${apiUrl}/screenComponents/createComponentRenderMetric`;
  
  // Get device info
  const deviceInfo = await getDeviceInfoForMetrics();
  
  const body = JSON.stringify({
    uniqueAccessCode,
    name,
    renderTime,
    deviceInfo, // Make sure this is included in the request body
  });

  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body,
    });
    
    // Add more detailed error logging
    if (!response.ok) {
      await saveMetric({ uniqueAccessCode, name, renderTime, deviceInfo });
    } else {
      console.log("Metric sent successfully!");
    }
  } catch (error) {
    console.log('Error sending metric:', error);
    // Log more details about the error
    if (error instanceof Error) {
      console.log('Error details:', error.message);
      console.log('Error stack:', error.stack);
    }
    await saveMetric({ uniqueAccessCode, name, renderTime, deviceInfo });
  }
};
  
  export const createNavigationMetric = async (uniqueAccessCode: string, fromScreen: string, toScreen: string, navigationCompletionTime: number) => {
    //console.log("body", uniqueAccessCode, fromScreen, toScreen, navigationCompletionTime);
    const url = `${apiUrl}/navigations/createNavigationMetric`;
    const deviceInfo = await getDeviceInfoForMetrics();

    const body = JSON.stringify({
      uniqueAccessCode,
      fromScreen,
      toScreen,
      navigationCompletionTime,
      deviceInfo
    });
  
    try {
      return await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body,
      }).then(handleResponse);
    } catch (error) {
      console.log('Error sending navigation metric:', error);
      await saveMetric({ uniqueAccessCode, fromScreen, toScreen, navigationCompletionTime, deviceInfo });
    }
  };

  export const createNetworkRequestMetric = async (uniqueAccessCode: string, name: string, requestCompletionTime: number) => {
    //console.log("body", uniqueAccessCode, name, requestCompletionTime);
    const url = `${apiUrl}/networkRequests/createNetworkRequestMetric`;

    const deviceInfo = await getDeviceInfoForMetrics();

    const body = JSON.stringify({
      uniqueAccessCode,
      name,
      requestCompletionTime,
      deviceInfo
    });
  
    try {
      return await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body,
      });

    } catch (error) {
      console.log('Error sending network request metric:', error);
      await saveMetric({ uniqueAccessCode, name, requestCompletionTime, deviceInfo });
      return new Response(null, { 
        status: 0,
        statusText: 'Network error occurred, saved for later retry' 
      });
    }
  };

  export const submitComponentCode = async (uniqueAccessCode: string, name: string, sourceCode: string) => {
    const url = `${apiUrl}/screenComponents/submitComponentCode`;
    const body = JSON.stringify({
      uniqueAccessCode,
      name,
      sourceCode,
    });
  
    try {
      return await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body,
      }).then(handleResponse);
    } catch (error) {
      console.log('Error sending component code:', error);
      // We could add offline storage for code too, but that might be excessive
    }
  };


  function handleResponse(response: Response) {
    if (!response.ok) {
      console.log(`Failed to send metric: ${response.statusText}`);
    }
    else{
      console.log('metric sent successfully');
    }
    
}