import { createNetworkRequestMetric } from "./api";

type FetchRequestData = {
    method: string;
    name: string;
    startTime: number;
    endTime?: number;
    duration?: number;
  };
  
  export const fetchWithTimer = async (uniqueAccessCode: string, name: string, url: string, options?: RequestInit): Promise<Response> => {
    const requestData: FetchRequestData = {
      method: options?.method || 'GET',
      name: name || 'Fetch request',
      startTime: performance.now(),
    };
  
    try {
      const response = await fetch(url, options);
      requestData.endTime = performance.now();
      requestData.duration = requestData.endTime - requestData.startTime;
      console.log(`Fetch request to ${requestData.name} took ${requestData.duration}ms`);
      const send = async () => {
        if (requestData.duration) {
        await createNetworkRequestMetric(uniqueAccessCode, requestData.name, requestData.duration);
        }
      }
      send();
      return response;
    } catch (error) {
      requestData.endTime = performance.now();
      requestData.duration = requestData.endTime - requestData.startTime;
      console.log(`Fetch request to ${requestData.name} failed after ${requestData.duration}ms`);
      throw error;
    }
  };