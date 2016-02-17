package solution;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.mapreduce.Reducer;



public class CanopyReducer extends Reducer<ClusterCenter, Vector, ClusterCenter, IntWritable> { 
	List<ClusterCenter> finalCenters = new LinkedList<ClusterCenter>();
	
	@Override
	protected void reduce(ClusterCenter key, Iterable<Vector> values, Context context) throws IOException, InterruptedException {
		ClusterCenter center = new ClusterCenter(key);
		CanopyJob.LOG.error("Start " + center.getCenter().getStockName() + " centers now " + finalCenters.size());
		boolean belongToCenter = false;
		double distance;
		
		for (ClusterCenter currCenter : finalCenters){ 
			
			distance = DistanceMeasurer.measureDistance(currCenter.getCenter(), center.getCenter()) / center.getCenter().getVector().length; 
			if (distance <= CanopyJob.T2) {
				belongToCenter = true;
				break;
			}
		}
		
		if (!belongToCenter){
			finalCenters.add(center);
			context.write(center, new IntWritable(0)); 
		}
	}
}
