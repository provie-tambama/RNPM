// src/performance/withRenderTimeMonitor.tsx
import React, { useEffect, useRef } from 'react';
import { createComponentRenderMetric, submitComponentCode } from '../utils/api';
import { getComponentSource } from '../utils/sourceRegistry';

const seenStartTimes = new Set<number>();

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
        const hasBeenMeasured = useRef<boolean>(false);
    // Ref to store the start time
    const startTimeRef = useRef<number>(performance.now());

    console.log(`Start time: ${startTimeRef.current}`);

    useEffect(() => {
      if (!hasBeenMeasured.current){
        const startTime = startTimeRef.current;
        const endTime = performance.now();

        if (seenStartTimes.has(startTime)) {
          console.log(`Skipping duplicate measurement with start time: ${startTime}`);
          return;
        }

        seenStartTimes.add(startTime);

        hasBeenMeasured.current = true;

        console.log(`End time: ${endTime}`);
        const renderTime = endTime - startTime;
        console.log(`Calculated render time: ${renderTime}ms`);
        const componentName = WrappedComponent.displayName || WrappedComponent.name || 'UnknownComponent';
        console.log(`Component name: ${componentName}`);

        if (onRenderTimeMeasured) {
          console.log('render time:', renderTime);
          onRenderTimeMeasured(renderTime);
        }


      const send = async () => {

        await createComponentRenderMetric(uniqueAccessCode, componentName, renderTime);

        if (captureCode) {
          try {
            // Get component source code
            console.log('Component name:', componentName);
            const sourceCode = getComponentSource(componentName);
            //console.log('Component source code:', sourceCode);
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
      }

      
    }, []);

    return <WrappedComponent {...(props as P)} />;
  };

  return WithRenderTimeMonitor;
};

export default withRenderTimeMonitor;
