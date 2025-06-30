import { createComponentRenderMetric, createNavigationMetric, createNetworkRequestMetric } from './api';
import { getMetrics, removeMetrics } from './offlineStorage';

const retryMetrics = async () => {
  const metrics = await getMetrics();

  for (const metric of metrics) {
    if (metric.renderTime !== undefined) {
      await createComponentRenderMetric(metric.uniqueAccessCode, metric.name, metric.renderTime);
    } else if (metric.navigationCompletionTime !== undefined) {
      await createNavigationMetric(metric.uniqueAccessCode, metric.fromScreen, metric.toScreen, metric.navigationCompletionTime);
    } else if (metric.requestCompletionTime !== undefined) {
      await createNetworkRequestMetric(metric.uniqueAccessCode, metric.name, metric.requestCompletionTime);
    }
  }
  //console.log("done retrying");
  // Clear the metrics after retryin
  await removeMetrics();
};

// Call retryMetrics periodically
export const initRetryMechanism = () => {
  //console.log("initRetryMechanism");
  setInterval(retryMetrics, 60000);
};
