// src/performance/withRenderTimeMonitor.tsx
import React, { useEffect, useRef } from 'react';
import { createComponentRenderMetric } from '../utils/api';

type WithRenderTimeMonitorProps = {
  uniqueAccessCode: string;
  onRenderTimeMeasured?: (time: number) => void;
};

const withRenderTimeMonitor = <P extends object>(
  WrappedComponent: React.ComponentType<P>
): React.FC<P & WithRenderTimeMonitorProps> => {
  const WithRenderTimeMonitor: React.FC<P & WithRenderTimeMonitorProps> = ({
    uniqueAccessCode,
    onRenderTimeMeasured,
    ...props
  }) => {
    const startTime = useRef<number>(performance.now());
    //console.log(`Start time: ${startTime.current}`);

    useEffect(() => {
      const endTime = performance.now();
      //console.log(`End time: ${endTime}`);
      const renderTime = endTime - startTime.current;
      //console.log(`Calculated render time: ${renderTime}ms`);
      console.log('Component mounted');
      if (onRenderTimeMeasured) {
        onRenderTimeMeasured(renderTime);
      }
      else {
      
      }
      console.log('Component mounted');
      const send = async () => {
        
        await createComponentRenderMetric(uniqueAccessCode, 'Home', renderTime);
      }
      send();
    }, []);

    return <WrappedComponent {...(props as P)} />;
  };

  return WithRenderTimeMonitor;
};

export default withRenderTimeMonitor;
