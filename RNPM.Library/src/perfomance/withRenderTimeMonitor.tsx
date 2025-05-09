// src/performance/withRenderTimeMonitor.tsx
import React, { useEffect, useRef } from 'react';
import { createComponentRenderMetric, submitComponentCode } from '../utils/api';
import { getComponentSource, getRegisteredComponents } from '../utils/sourceRegistry';

type WithRenderTimeMonitorProps = {
  uniqueAccessCode: string;
  onRenderTimeMeasured?: (time: number) => void;
  captureCode?: boolean;
};

const withRenderTimeMonitor = <P extends object>(
  WrappedComponent: React.ComponentType<P>
): React.FC<P & WithRenderTimeMonitorProps> => {
  const WithRenderTimeMonitor: React.FC<P & WithRenderTimeMonitorProps> = ({
    uniqueAccessCode,
    onRenderTimeMeasured,
    captureCode = true,
    ...props
  }) => {
    const startTime = useRef<number>(performance.now());
    //console.log(`Start time: ${startTime.current}`);

    useEffect(() => {
      const endTime = performance.now();
      //console.log(`End time: ${endTime}`);
      const renderTime = endTime - startTime.current;
      //console.log(`Calculated render time: ${renderTime}ms`);
      const componentName = WrappedComponent.displayName || WrappedComponent.name || 'UnknownComponent';

      if (onRenderTimeMeasured) {
        onRenderTimeMeasured(renderTime);
      }

      const send = async () => {

        await createComponentRenderMetric(uniqueAccessCode, componentName, renderTime);
        
        if (captureCode) {
          try {
            // Get component source code
            const sourceCode = getComponentSource(componentName);
            if (sourceCode) {
              await submitComponentCode(uniqueAccessCode, componentName, sourceCode);
            }
          } catch (error) {
            console.error('Error capturing component code:', error);
          }
        }
        else{
          console.log('Component code capture skipped');
        }

      }
      send();
    }, []);

    return <WrappedComponent {...(props as P)} />;
  };

  return WithRenderTimeMonitor;
};

export default withRenderTimeMonitor;
