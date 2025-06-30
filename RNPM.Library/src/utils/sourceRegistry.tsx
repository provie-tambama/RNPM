type SourceEntry = {
    name: string;
    source: string;
    path?: string;
  };

  const sourceRegistry: Record<string, SourceEntry> = {};
  
  export function registerComponentSource(name: string, source: string, path?: string) {
    sourceRegistry[name] = { name, source, path };
  }
  
  export function getComponentSource(name: string): string | null {
    return sourceRegistry[name]?.source || null;
  }
  
  export function getRegisteredComponents(): SourceEntry[] {
    return Object.values(sourceRegistry);
  }
  
if (typeof global !== 'undefined') {
  (global as any).__RNPM_registerComponentSource = registerComponentSource;
  
  const queue = (global as any).__RNPM_registerComponentSource_queue || [];
  if (queue.length > 0) {
    queue.forEach((item: {name: string, source: string, path?: string}) => {
      registerComponentSource(item.name, item.source, item.path);
    });
    queue.length = 0;
  }
}