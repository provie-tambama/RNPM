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
  }