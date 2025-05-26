// src/utils/sourceRegistry.ts
type SourceEntry = {
    name: string;
    source: string;
    path?: string;
  };
  
  // Registry to store component source code
  const sourceRegistry: Record<string, SourceEntry> = {};
  
  /**
   * Register component source code
   */
  export function registerComponentSource(name: string, source: string, path?: string) {
    sourceRegistry[name] = { name, source, path };
  }
  
  /**
   * Get component source code
   */
  export function getComponentSource(name: string): string | null {
    return sourceRegistry[name]?.source || null;
  }
  
  /**
   * List all registered components
   */
  export function getRegisteredComponents(): SourceEntry[] {
    return Object.values(sourceRegistry);
  }
  
  // Make the registry globally available for the Babel plugin
if (typeof global !== 'undefined') {
  (global as any).__RNPM_registerComponentSource = registerComponentSource;
  
  // Process any queued registrations
  const queue = (global as any).__RNPM_registerComponentSource_queue || [];
  if (queue.length > 0) {
    queue.forEach((item: {name: string, source: string, path?: string}) => {
      registerComponentSource(item.name, item.source, item.path);
    });
    // Clear the queue
    queue.length = 0;
  }
}