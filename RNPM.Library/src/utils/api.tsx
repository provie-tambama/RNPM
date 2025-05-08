import {apiUrl} from '../constants/api';
import { saveMetric } from './offlineStorage';

export const createComponentRenderMetric = async (uniqueAccessCode: string, name: string, renderTime: number) => {
  console.log("body", uniqueAccessCode, name, renderTime)
    const url = `${apiUrl}/screenComponents/createComponentRenderMetric`;
    const body = JSON.stringify({
      uniqueAccessCode,
      name,
      renderTime,
    });
  
    try {
       const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body,
      });
      if (!response.ok) {
        console.log(`HTTP error! status: ${response.statusText}`);
        await saveMetric({ uniqueAccessCode, name, renderTime });
      }
    } catch (error) {
      console.log('Error sending metric:', error);
      await saveMetric({ uniqueAccessCode, name, renderTime });
    }
  };
  
  export const createNavigationMetric = async (uniqueAccessCode: string, fromScreen: string, toScreen: string, navigationCompletionTime: number) => {
    //console.log("body", uniqueAccessCode, fromScreen, toScreen, navigationCompletionTime);
    const url = `${apiUrl}/navigations/createNavigationMetric`;
    const body = JSON.stringify({
      uniqueAccessCode,
      fromScreen,
      toScreen,
      navigationCompletionTime,
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
      await saveMetric({ uniqueAccessCode, fromScreen, toScreen, navigationCompletionTime });
    }
  };

  export const createNetworkRequestMetric = async (uniqueAccessCode: string, name: string, requestCompletionTime: number) => {
    //console.log("body", uniqueAccessCode, name, requestCompletionTime);
    const url = `${apiUrl}/networkRequests/createNetworkRequestMetric`;
    const body = JSON.stringify({
      uniqueAccessCode,
      name,
      requestCompletionTime,
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
      await saveMetric({ uniqueAccessCode, name, requestCompletionTime });
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