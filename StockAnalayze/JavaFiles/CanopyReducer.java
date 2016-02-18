package solution;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Comparator;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.SequenceFile;
import org.apache.hadoop.mapreduce.Reducer;



public class CanopyReducer extends Reducer<ClusterCenter, Vector, ClusterCenter, IntWritable> { 
    Map<ClusterCenter,Integer> finalCenters = new HashMap<ClusterCenter,Integer>();
    
    @Override
    protected void reduce(ClusterCenter key, Iterable<Vector> values, Context context) throws IOException, InterruptedException {
        ClusterCenter center = new ClusterCenter(key);
        boolean belongToCenter = false;
        double distance;
        
        for (Map.Entry<ClusterCenter, Integer> entry : finalCenters.entrySet()) {
            ClusterCenter currCenter = entry.getKey();
            distance = DistanceMeasurer.measureDistance(currCenter.getCenter(), center.getCenter()) / center.getCenter().getVector().length; 
            if (distance <= JobRunner.T2) {
                belongToCenter = true;
                finalCenters.put(currCenter,finalCenters.get(currCenter) + 1);
                break;
            }
        }
        
        if (!belongToCenter){
            finalCenters.put(center,0);
            context.write(center, new IntWritable(0)); 
        }
    }

	@Override
	protected void cleanup(Context context) throws IOException, InterruptedException {    
	    super.cleanup(context);
	    
	    Object[] sortedMap = finalCenters.entrySet().toArray();
	    Arrays.sort(sortedMap, new Comparator<Object>() {
			@Override
			public int compare(Object o1, Object o2) {
	            return ((Map.Entry<ClusterCenter, Integer>) o2).getValue().compareTo(
	                    ((Map.Entry<ClusterCenter, Integer>) o1).getValue());
			}
		});
	    
	    Configuration conf = context.getConfiguration();
	    int k = Integer.parseInt(conf.get("k"));
	    
	    List<ClusterCenter> listToWrite = new ArrayList<ClusterCenter>();
	    
	    if (finalCenters.size() > k) {
			Iterator<Entry<ClusterCenter,Integer>> iterator = finalCenters.entrySet().iterator();
			while (iterator.hasNext() && listToWrite.size() < k) {
				Map.Entry<ClusterCenter,Integer> entry = (Map.Entry<ClusterCenter,Integer>) iterator.next();
				listToWrite.add(entry.getKey());
			}
	    } else {
		    int totalNumOfPoints = 0;

		    for (Map.Entry<ClusterCenter, Integer> entry : finalCenters.entrySet()) {
		        totalNumOfPoints += entry.getValue();
		        listToWrite.add(entry.getKey());
		    }
		    
		    double clusterSize = totalNumOfPoints / k;
		    k -= finalCenters.size();
		    
		    for (int i=0;k > 0 && i<sortedMap.length;i++) {
		    	Map.Entry<ClusterCenter, Integer> entry = (Map.Entry<ClusterCenter, Integer>)sortedMap[i];
		    	
		        double centersInCanopy = entry.getValue() / clusterSize;
		        int createMoreCenters = (int)Math.round(centersInCanopy);
		        while (k > 0 && createMoreCenters > 1) {
		        	JobRunner.LOG.error("ADD MORE ");
		        	listToWrite.add(getRandomCenter(entry.getKey()));
		        	createMoreCenters--;
		        	k--;
		        }
		    }
		    
		    while (k > 0) {
		    	Map.Entry<ClusterCenter, Integer> entry = (Map.Entry<ClusterCenter, Integer>)sortedMap[0];
		    	ClusterCenter firstKey = entry.getKey();
		    	listToWrite.add(firstKey);
		    	k--;
		    }
	    }
	    
	    SequenceFile.Writer centersWriter = null ;
		
	    try {
	        FileSystem fs = FileSystem.get(conf);
	        Path centersFile = new Path(conf.get("centroid.path"));
	    	
	        centersWriter =  SequenceFile.createWriter(fs, conf, centersFile, ClusterCenter.class, IntWritable.class);
	        
	        for (ClusterCenter currCenter : listToWrite){
	        	centersWriter.append(currCenter, new IntWritable(0));
	        }
	    } catch (IOException e) {
	        e.printStackTrace();
	    } finally {
	    	if (centersWriter != null) {
	    		centersWriter.close();
	    	}
	    }
	}
	
	public static ClusterCenter getRandomCenter(ClusterCenter center){
		
		double rand;
		double[] point = center.getCenter().getVector();
		double[] newPoints = new double[point.length];
		for (int i = 0; i < point.length; i++){
			rand = Math.random() * 10;
			newPoints[i] = point[i] + rand;			
		}
		
		Vector newVector = new Vector();
		newVector.setStockName("GEN");
		newVector.setVector(newPoints);
		return new ClusterCenter(newVector);
	}
}