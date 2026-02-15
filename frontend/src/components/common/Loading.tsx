interface LoadingProps {
  size?: 'sm' | 'md' | 'lg';
  fullScreen?: boolean;
  text?: string;
}

const Loading: React.FC<LoadingProps> = ({ 
  size = 'md', 
  fullScreen = false,
  text
}) => {
  const sizes = {
    sm: 'h-8 w-8',
    md: 'h-12 w-12',
    lg: 'h-16 w-16',
  };
  
  const spinner = (
    <div className="flex flex-col items-center justify-center gap-3">
      <div className={`animate-spin rounded-full border-b-2 border-blue-600 ${sizes[size]}`} />
      {text && <p className="text-gray-600 text-sm">{text}</p>}
    </div>
  );
  
  if (fullScreen) {
    return (
      <div className="fixed inset-0 flex items-center justify-center bg-white bg-opacity-75 z-50">
        {spinner}
      </div>
    );
  }
  
  return <div className="flex items-center justify-center p-8">{spinner}</div>;
};

export default Loading;
