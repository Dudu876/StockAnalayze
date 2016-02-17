package solution;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Mapper;


public class CanopyMapper extends Mapper<LongWritable, Text, ClusterCenter, Vector> {
	static enum CountersEnum { MAP_COUNT }
	static enum CounterEnum { DIST_COUNT }
	static enum SumEnum { MAP_SUM }
	List<ClusterCenter> centers = new LinkedList<ClusterCenter>();
	
	@Override
	protected void map(LongWritable key, Text value, Context context) throws IOException, InterruptedException	{ // finding canopy centers
		
		
		context.getCounter(CountersEnum.MAP_COUNT).increment(1);
		
		Vector vector = new Vector(value);
		CanopyJob.LOG.error("Start " + vector.getStockName() + " centers now " + centers.size());
		boolean belongToCenter = false;
		double distance;
		for (ClusterCenter currCenter : centers){
			
			distance = DistanceMeasurer.measureDistance(currCenter.getCenter(), vector) / vector.getVector().length;
			context.getCounter(CounterEnum.DIST_COUNT).increment(1);
			context.getCounter(SumEnum.MAP_SUM).increment((long) distance);
			if (distance <= CanopyJob.T2) {
				belongToCenter = true;
				break;
			} else if (distance <= CanopyJob.T1) {
				context.write(new ClusterCenter(vector), vector);
				belongToCenter = true;
				break;
			}
		}
		
		if (!belongToCenter){
			centers.add(new ClusterCenter(vector));
			context.write(new ClusterCenter(vector), vector);
		}
	}
}
